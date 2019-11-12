using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Edm;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData.UriParser;
using Microsoft.AspNetCore.Cors.Infrastructure;
using EJ2APIServices.Models;
using EJ2APIServices.Controllers;
using Syncfusion.EJ2.SpellChecker;

namespace EJ2APIServices
{
    public class Startup
    {
        private string _contentRootPath = "";
        internal static List<SpellCheckDictionary> spellDictCollection;
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            _contentRootPath = env.ContentRootPath;
            ServerPath.MapPath = _contentRootPath;
            string dictionaryPath = env.ContentRootPath + "\\App_Data\\en_US.dic";
            string affixPath = env.ContentRootPath + "\\App_Data\\en_US.aff";
            string customDict = env.ContentRootPath + "\\App_Data\\customDict.dic";
            List<DictionaryData> items = new List<DictionaryData>() {
               new DictionaryData(1046,dictionaryPath,affixPath,customDict),
            };
            spellDictCollection = new List<SpellCheckDictionary>();
            foreach (var item in items)
            {
                spellDictCollection.Add(new SpellCheckDictionary(new DictionaryData(1046,dictionaryPath,affixPath,customDict)));
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOData();
            services.AddMvc().AddJsonOptions(x => {
                x.SerializerSettings.ContractResolver = null;
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });
            string connection = Configuration.GetConnectionString("Test");
            if (connection.Contains("%CONTENTROOTPATH%"))
            {
                connection = connection.Replace("%CONTENTROOTPATH%", _contentRootPath);

            }
            services.AddEntityFrameworkNpgsql().AddDbContext<EEJ2SERVICEEJ2WEBSERVICESSRCAPP_DATADIAGRAMMDFContext>(options => options.UseNpgsql(connection));
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseCors("AllowAllOrigins");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(b =>
            {
                b.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
            });

        }

    }

    public class ServerPath
    {
        public static string MapPath = "";
    }
}
