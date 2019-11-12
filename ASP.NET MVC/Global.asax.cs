using Syncfusion.EJ2.SpellChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Routing;

namespace EJ2DocumentEditorAPIServices
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        internal static List<SpellCheckDictionary> spellDictCollection;
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            string dictionaryPath = HostingEnvironment.MapPath("\\App_Data\\en_US.dic");
            string affixPath = HostingEnvironment.MapPath("\\App_Data\\en_US.aff");
            string customDict = HostingEnvironment.MapPath("\\App_Data\\customDict.dic");
            List<DictionaryData> items = new List<DictionaryData>() {
               new DictionaryData(1046,dictionaryPath,affixPath,customDict),
            };
            spellDictCollection = new List<SpellCheckDictionary>();
            foreach (var item in items)
            {
                spellDictCollection.Add(new SpellCheckDictionary(item));
            }
        }
    }
}
