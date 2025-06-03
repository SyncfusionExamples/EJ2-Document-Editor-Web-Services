# Integrated .Net Core Library in python

We have created Documenteditor utility with following functionalities:

* [Import](../README.md/#import)
* [ExportSfdt](../README.md/#exportsfdt)
* [SystemClipboard](../README.md/#systemclipboard)
* [RestrictEditing](../README.md/#restrictediting)


Document editor does not have native support to open a document using Python. We can use the .NET Core Wrapper as a Python backend.

1. **Install required `dependencies`:**
  ```
    python -m pip install flask
    python -m pip install flask-cors
    python -m pip install pythonnet
  ```

2. **Run (buildNetProject.bat) bat file**
3. **Open cmd and run `"python app.py"`**


Run the sample and set the serviceUrl as the running URL.

**For example,**
container.serviceUrl=`'http://127.0.0.1:5000/'`;

>**Note :** This is a development server. It should not be utilized for production deployment. Instead, employ a production-grade WSGI server.