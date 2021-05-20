using Syncfusion.EJ2.SpellChecker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using EJ2WordDocument = Syncfusion.EJ2.DocumentEditor.WordDocument;

namespace EJ2DocumentEditorWebServices.Controllers
{

    [EnableCors("*", "*", "*")]
    [RoutePrefix("api/documenteditor")]
    public class DocumentEditorController : ApiController
    {
        List<DictionaryData> spellDictionary;
        string personalDictPath;
        public DocumentEditorController()
        {
            spellDictionary = WebApiApplication.spellDictCollection;
            personalDictPath = WebApiApplication.personalDictPath;
        }

        [HttpPost]
        [EnableCors("*", "*", "*")]
        [Route("Import")]
        public HttpResponseMessage Import()
        {
            if (HttpContext.Current.Request.Files.Count == 0)
                return null;

            HttpPostedFile file = HttpContext.Current.Request.Files[0];
            int index = file.FileName.LastIndexOf('.');
            string type = index > -1 && index < file.FileName.Length - 1 ?
                file.FileName.Substring(index) : ".docx";
            Stream stream = file.InputStream;
            stream.Position = 0;

            EJ2WordDocument document = EJ2WordDocument.Load(stream, GetFormatType(type.ToLower()));
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            return new HttpResponseMessage() { Content = new StringContent(json) };
        }

        [HttpPost]
        [EnableCors("*", "*", "*")]
        [Route("SystemClipboard")]
        public HttpResponseMessage SystemClipboard([FromBody] CustomParameter param)
        {
            if (param.content != null && param.content != "")
            {
                try
                {
                    Syncfusion.EJ2.DocumentEditor.WordDocument document = Syncfusion.EJ2.DocumentEditor.WordDocument.LoadString(param.content, GetFormatType(param.type.ToLower()));
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(document);
                    document.Dispose();
                    return new HttpResponseMessage() { Content = new StringContent(json) };
                }
                catch (Exception)
                {
                    return new HttpResponseMessage() { Content = new StringContent("") };
                }

            }
            return new HttpResponseMessage() { Content = new StringContent("") };
        }

        [HttpPost]
        [EnableCors("*", "*", "*")]
        [Route("RestrictEditing")]
        public string[] RestrictEditing([FromBody] CustomRestrictParameter param)
        {
            if (param.passwordBase64 == "" && param.passwordBase64 == null)
                return null;
            return Syncfusion.EJ2.DocumentEditor.WordDocument.ComputeHash(param.passwordBase64, param.saltBase64, param.spinCount);
        }

        [HttpPost]
        [EnableCors("*", "*", "*")]
        [Route("SpellCheck")]
        public HttpResponseMessage SpellCheck([FromBody] SpellCheckJsonData spellChecker)
        {
            try
            {
                SpellChecker spellCheck = new SpellChecker(spellDictionary, personalDictPath);
                spellCheck.GetSuggestions(spellChecker.LanguageID, spellChecker.TexttoCheck, spellChecker.CheckSpelling, spellChecker.CheckSuggestion, spellChecker.AddWord);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(spellCheck);
                return new HttpResponseMessage() { Content = new StringContent(json) };
            }
            catch
            {
                return new HttpResponseMessage() { Content = new StringContent("{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}") };
            }
        }

        [HttpPost]
        [EnableCors("*", "*", "*")]
        [Route("SpellCheckByPage")]
        public HttpResponseMessage SpellCheckByPage([FromBody] SpellCheckJsonData spellChecker)
        {
            try
            {
                SpellChecker spellCheck = new SpellChecker(spellDictionary, personalDictPath);
                spellCheck.CheckSpelling(spellChecker.LanguageID, spellChecker.TexttoCheck);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(spellCheck);
                return new HttpResponseMessage() { Content = new StringContent(json) };
            }
            catch
            {
                return new HttpResponseMessage() { Content = new StringContent("{\"SpellCollection\":[],\"HasSpellingError\":false,\"Suggestions\":null}") };
            }
        }

        internal static Syncfusion.EJ2.DocumentEditor.FormatType GetFormatType(string format)
        {
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            switch (format.ToLower())
            {
                case ".dotx":
                case ".docx":
                case ".docm":
                case ".dotm":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Docx;
                case ".dot":
                case ".doc":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Doc;
                case ".rtf":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Rtf;
                case ".txt":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Txt;
                case ".xml":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.WordML;
                case ".html":
                    return Syncfusion.EJ2.DocumentEditor.FormatType.Html;
                default:
                    throw new NotSupportedException("EJ2 DocumentEditor does not support this file format.");
            }
        }

        public string ImportWordDocument(Stream stream)
        {
            string sfdtText = "";
            EJ2WordDocument document = EJ2WordDocument.Load(stream, Syncfusion.EJ2.DocumentEditor.FormatType.Docx);
            sfdtText = Newtonsoft.Json.JsonConvert.SerializeObject(document);
            document.Dispose();
            return sfdtText;
        }
    }
    public class SpellCheckJsonData
    {
        public int LanguageID { get; set; }
        public string TexttoCheck { get; set; }
        public bool CheckSpelling { get; set; }
        public bool CheckSuggestion { get; set; }
        public bool AddWord { get; set; }

    }
    public class CustomParameter
    {
        public string content { get; set; }
        public string type { get; set; }
    }

    public class CustomRestrictParameter
    {
        public string passwordBase64 { get; set; }
        public string saltBase64 { get; set; }
        public int spinCount { get; set; }
    }

}