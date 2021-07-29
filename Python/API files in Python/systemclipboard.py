import clr #import clr from pythonnet
import base64

#load our dll file(mine is in my C:\\ folder)
clr.AddReference("**\\Python\\DocumentEditorLibrary\\DocumentEditorLibrary\\bin\\Debug\\netstandard2.0\\DocumentEditorLibrary.dll")
clr.AddReference("C:\\Users\\Admin\\.nuget\\packages\\syncfusion.ej2.wordeditor.aspnet.core\\19.2.0.49\\lib\\netstandard2.0\\Syncfusion.EJ2.DocumentEditor.dll")
clr.AddReference("C:\\Users\\Admin\\.nuget\\packages\\syncfusion.docio.net.core\\19.2.0.49\\lib\\netstandard2.0\\Syncfusion.DocIO.Portable.dll")
clr.AddReference("C:\\Users\\Admin\\.nuget\\packages\\syncfusion.compression.net.core\\19.2.0.49\\lib\\netstandard2.0\\Syncfusion.Compression.Portable.dll")
clr.AddReference("C:\\Users\\Admin\\.nuget\\packages\\syncfusion.officechart.net.core\\19.2.0.49\\lib\\netstandard2.0\\Syncfusion.OfficeChart.Portable.dll")
clr.AddReference("C:\\Users\\Admin\\.nuget\\packages\\syncfusion.licensing\\19.2.0.49\\lib\\netstandard2.0\\Syncfusion.Licensing.dll")
clr.AddReference("C:\\Program Files\\dotnet\\sdk\\NuGetFallbackFolder\\newtonsoft.json\\10.0.1\\lib\\net20\\Newtonsoft.Json.dll")

#import our Documenteditor class from Our C# namespace DocumentEditorLibrary
from DocumentEditorLibrary import Documenteditor

docEditor = Documenteditor() #create our Documenteditor object

content="";
#calling our import method with base64String of parameter and printing
print("Sfdt for provided base64String: "+str(docEditor.SystemClipboard(content,".html")))

