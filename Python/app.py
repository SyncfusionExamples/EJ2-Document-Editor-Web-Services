from flask import Flask, request, send_file
from flask_cors import CORS
from io import BytesIO
import os
import json

# Configure Python.NET to use the .NET (CoreCLR) runtime before importing clr.
# Without this, pythonnet may fall back to the legacy Mono/.NET Framework runtime,
# which cannot load net10.0 assemblies and the wrapper class import fails.
os.environ.setdefault("PYTHONNET_RUNTIME", "coreclr")
import clr

app = Flask(__name__)

# Raise the request size limit so large Word documents can be imported.
app.config['MAX_CONTENT_LENGTH'] = 500 * 1024 * 1024      # 500 MB
app.config['MAX_FORM_MEMORY_SIZE'] = 500 * 1024 * 1024    # 500 MB

# Enable CORS so the React application can call the service.
CORS(app)

# Point this to the publish output folder of the class library.
publish_base = os.path.join(
    os.getcwd(), "dotnet", "DocumentEditorLibrary", "bin", "Release", "net10.0", "publish"
)

# Load the wrapper library and the Syncfusion assemblies.
clr.AddReference(os.path.join(publish_base, "DocumentEditorLibrary.dll"))
clr.AddReference(os.path.join(publish_base, "Syncfusion.EJ2.DocumentEditor.dll"))
clr.AddReference(os.path.join(publish_base, "Syncfusion.DocIO.Portable.dll"))
clr.AddReference(os.path.join(publish_base, "Syncfusion.Compression.Portable.dll"))
clr.AddReference(os.path.join(publish_base, "Syncfusion.OfficeChart.Portable.dll"))
clr.AddReference(os.path.join(publish_base, "Newtonsoft.Json.dll"))
clr.AddReference(os.path.join(publish_base, "SkiaSharp.dll"))
clr.AddReference(os.path.join(publish_base, "BitMiracle.LibTiff.NET.dll"))

# Import the wrapper class from the C# namespace.
from DocumentEditorLibrary import Documenteditor

docEditor = Documenteditor()   # create the wrapper object

# The Import API loads a Word document into the DOCX Editor by
# converting it to SFDT. The MetafileImageParsed event converts
# TIFF, EMF, and WMF images to raster images because web browsers
# cannot display metafile images.
@app.route('/Import', methods=['POST'])
def importDocument():
    if 'files' in request.files:
        file = request.files['files']
        # Get the stream data and the file name.
        stream_data = file.stream.read()
        file_name = file.filename
        # Returns the SFDT string for the uploaded Word document.
        return docEditor.Import(stream_data, file_name)
    return ""

# The SystemClipboard API converts clipboard data (HTML/RTF) to SFDT
# so content can be pasted with formatting.
@app.route('/SystemClipboard', methods=['POST'])
def systemClipboard():
    # Get the SFDT data and the type from the request.
    content = request.json['content']
    type = request.json['type']
    # Returns the SFDT string for the clipboard content.
    return docEditor.SystemClipboard(content, type)

# The RestrictEditing API computes the password hash used by the
# document protection feature.
@app.route('/RestrictEditing', methods=['POST'])
def restrictEditing():
    passwordBase64 = request.json['passwordBase64']
    saltBase64 = request.json['saltBase64']
    spinCount = request.json['spinCount']
    # Returns an array of strings that represent the password and salt values.
    jsonString = docEditor.RestrictEditing(passwordBase64, saltBase64, spinCount)
    return json.dumps(list(jsonString))

# The Save API exports the edited document from the DOCX Editor.
@app.route('/Save', methods=['POST'])
def save():
    # Get the SFDT data and the file name from the request.
    content = request.json['content']
    fileName = request.json['fileName']
    # Saves the SFDT content as a Word document and returns it.
    result = docEditor.Save(content, fileName)
    output = BytesIO(bytes(result))
    output.seek(0)
    return send_file(output, as_attachment=True, download_name=fileName)

# The MailMerge API performs mail merge on the uploaded Word document
# and returns the merged document as SFDT.
@app.route('/MailMerge', methods=['POST'])
def mailMerge():
    # Get the base64 Word document from the request.
    documentData = request.json['documentData']
    # Performs mail merge and returns the merged document as SFDT.
    return docEditor.MailMerge(documentData)

# The CompareDocuments API compares the original and revised Word
# documents and returns the comparison result as SFDT.
@app.route('/CompareDocuments', methods=['POST'])
def compareDocuments():
    # Get the original and revised files from the request.
    originalFile = request.files['originalFile'].stream.read()
    revisedFile = request.files['revisedFile'].stream.read()
    # Returns the comparison result as SFDT.
    return docEditor.CompareDocuments(originalFile, revisedFile)

if __name__ == "__main__":
    app.run(debug=True)   # http://localhost:5000/