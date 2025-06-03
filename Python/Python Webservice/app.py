from flask import Flask, json, request
from flask_cors import CORS #import CORS from flask_cors

import clr #import clr from pythonnet
import os

app = Flask(__name__)
CORS(app) #enable CORS on the app

# get the current working directory
current_working_directory = os.getcwd()

#load our dll file(mine is in my C:/ folder)
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/DocumentEditorLibrary.dll")
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/Syncfusion.EJ2.DocumentEditor.dll")
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/Syncfusion.DocIO.Portable.dll")
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/Syncfusion.Compression.Portable.dll")
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/Syncfusion.OfficeChart.Portable.dll")
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/Syncfusion.Licensing.dll")
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/Newtonsoft.Json.dll")
clr.AddReference(current_working_directory + "/.NET Standard Wrapper Library/DocumentEditorLibrary/bin/Release/netstandard2.0/publish/System.Text.Encoding.CodePages.dll")

#import our Documenteditor class from our C# namespace DocumentEditorLibrary
from DocumentEditorLibrary import Documenteditor
from Syncfusion.Licensing import SyncfusionLicenseProvider

# Register Syncfusion license
SyncfusionLicenseProvider.RegisterLicense("Enter your license key here")

docEditor = Documenteditor() #create our Documenteditor object

@app.route('/Import', methods=['POST'])
def importDocument():
    if 'files' in request.files:
        files = request.files['files']
        # Get the stream data
        stream_data = files.stream.read()
        # Get the file name
        file_name = files.filename
        # Calling our Import method from our Documenteditor class which will return the SFDT string
        return docEditor.Import(stream_data, file_name)
    else:
        return ""

@app.route('/SystemClipboard', methods=['POST'])
def systemClipboard():
    # Get the SFDT data from the request
    content = request.json['content']
    # Get the type from the request
    type = request.json['type']
    # Calling our SystemClipboard method from our Documenteditor class which will return the SFDT string
    return docEditor.SystemClipboard(content, type)

@app.route('/RestrictEditing', methods=['POST'])
def restrictEditing():
    passwordBase64 = request.json['passwordBase64']
    slatBase64 = request.json['saltBase64']
    spinCount = request.json['spinCount']
    # Calling our RestrictEditing method from our Documenteditor class which will return the array of System.String represents the password and salt value.
    jsonString = docEditor.RestrictEditing(passwordBase64, slatBase64, spinCount)
    return json.loads(jsonString)

@app.route('/Save', methods=['POST'])
def save():
    # Get the SFDT data from the request
    content = request.json['content']
    # Get the file name from the request
    fileName = request.json['fileName']
    # Calling our Save method from our Documenteditor class which will save the document in the given file name.
    result = docEditor.Save(content, fileName)
    print(result)
    return result


@app.route("/")
def home():
    return "Flask Web API for DocumentEditor!"

if __name__ == "__main__":
    app.run(debug=True) # http://localhost:5000/
    # app.run(host='', port=5001, debug=True) # http://localhost:5001/
