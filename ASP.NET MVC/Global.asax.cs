using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Syncfusion.EJ2.SpellChecker;
using System.Web.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace EJ2DocumentEditorWebServices
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        internal static List<DictionaryData> spellDictCollection;
        internal static string path;
        internal static string personalDictPath;
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            //check the spell check dictionary path environment variable value and assign default data folder
            //if it is null.
            string path = HostingEnvironment.MapPath("\\App_Data\\");
            //Set the default spellcheck.json file if the json filename is empty.
            string jsonFileName = HostingEnvironment.MapPath("\\App_Data\\spellcheck.json");        
            if (System.IO.File.Exists(jsonFileName))
            {
                string jsonImport = System.IO.File.ReadAllText(jsonFileName);
                List<DictionaryData> spellChecks = JsonConvert.DeserializeObject<List<DictionaryData>>(jsonImport);
                spellDictCollection = new List<DictionaryData>();
                //construct the dictionary file path using customer provided path and dictionary name
                foreach (var spellCheck in spellChecks)
                {
                    spellDictCollection.Add(new DictionaryData(spellCheck.LanguadeID, Path.Combine(path, spellCheck.DictionaryPath), Path.Combine(path, spellCheck.AffixPath)));
                    personalDictPath = Path.Combine(path, spellCheck.PersonalDictPath);
                }
            }
        }
    }
}
