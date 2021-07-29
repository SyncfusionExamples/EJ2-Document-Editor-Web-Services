using Syncfusion.EJ2.DocumentEditor;
using System;
using System.IO;

namespace DocumentEditorLibrary
{
    public class Documenteditor
    {
        public string Import(string base64String)
        {

            byte[] byteArray = Convert.FromBase64String(base64String);
            Stream stream = new MemoryStream(byteArray);


            try
            {
                //Stream stream = System.IO.File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                WordDocument document = WordDocument.Load(stream, FormatType.Docx); ;
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                document.Dispose();
                stream.Dispose();
                return json;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SystemClipboard(string Content, string type)
        {
            if (Content != null && Content != "")
            {
                try
                {

                    WordDocument document = WordDocument.LoadString(Content, GetFormatType(type));
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                    document.Dispose();
                    return json;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            return "";
        }
        public string ExportSFDT(string content)
        {
            Stream document = WordDocument.Save(content, FormatType.Docx);
            FileStream file = new FileStream("sample.docx", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            document.CopyTo(file);
            file.Close();
            document.Close();
            return "success";
        }

        public string[] RestrictEditing(string passwordBase64, string saltBase64, int spinCount)
        {
            if (passwordBase64 == "" && passwordBase64 == null)
                return null;
            return WordDocument.ComputeHash(passwordBase64, saltBase64, spinCount);
        }

        internal static FormatType GetFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case ".dotx":
                case ".docx":
                case ".docm":
                case ".dotm":
                    return FormatType.Docx;
                case ".dot":
                case ".doc":
                    return FormatType.Doc;
                case ".rtf":
                    return FormatType.Rtf;
                case ".txt":
                    return FormatType.Txt;
                case ".xml":
                    return FormatType.WordML;
                case ".html":
                    return FormatType.Html;
                default:
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }
    }
}
