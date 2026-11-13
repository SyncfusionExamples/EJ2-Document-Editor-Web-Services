# Python backend web service sample for the DOCX Editor

This folder contains the tested wrapper files for hosting the DOCX Editor server-side operations from a Python (Flask) web service. See the documentation page [How to Use Python as a Backend Web Service in DOCX Editor](../use-python-as-backend-web-service.md) for the full walkthrough.

## Folder structure

| File | Purpose |
|---|---|
| `.NET Class Library/DocumentEditorLibrary/Documenteditor.cs` | C# wrapper class (`Import`, `Save`, `SystemClipboard`, `RestrictEditing`, `MailMerge`, `CompareDocuments`) |
| `.NET Class Library/DocumentEditorLibrary/DocumentEditorLibrary.csproj` | Class library project targeting `net10.0` with the required NuGet packages |
| `app.py` | Flask application hosting all six endpoints in a single file |

N> Keep the class library inside the `.NET Class Library` subfolder. If the `DocumentEditorLibrary` folder is placed directly next to `app.py`, Python treats it as a local package and the `from DocumentEditorLibrary import Documenteditor` call fails.

## Build the class library

```bash
cd .NET Class Library/DocumentEditorLibrary
.NET Class Library build -c Release
.NET Class Library publish -c Release
cd ../..
```

## Start the Python web service

```bash
pip install flask flask-cors pythonnet
python app.py
```

The service runs at `http://127.0.0.1:5000/`. Configure the DOCX Editor's `serviceUrl` property to this address.

On Linux/macOS, if pythonnet cannot find the .NET runtime, set `PYTHONNET_RUNTIME=coreclr` before starting the service.
