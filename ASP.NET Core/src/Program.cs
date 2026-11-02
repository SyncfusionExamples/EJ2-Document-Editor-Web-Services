using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.EJ2.SpellChecker;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Syncfusion.Licensing;
using Syncfusion.Telemetry;
using Microsoft.AspNetCore.Http;

namespace EJ2APIServices
{
    public class Program
    {
        internal static string path;
        
        public static void Main(string[] args)
        {
            // Disable Syncfusion telemetry data collection.
            Telemetry.Disable();
            
            var builder = WebApplication.CreateBuilder(args);

            var MyAllowSpecificOrigins = "AllowAllOrigins";

            var configuration = builder.Configuration;
            var env = builder.Environment;

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            builder.Services.AddMemoryCache();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins, policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = System.IO.Compression.CompressionLevel.Optimal;
            });

            builder.Services.AddResponseCompression();

            var app = builder.Build();
            path = configuration["SPELLCHECK_DICTIONARY_PATH"];
            string jsonFileName = configuration["SPELLCHECK_JSON_FILENAME"];
            int cacheCount = int.TryParse(configuration["SPELLCHECK_CACHE_COUNT"], out int result) ? result : 1;

            path = string.IsNullOrEmpty(path) ? Path.Combine(env.ContentRootPath, "Data") : Path.Combine(env.ContentRootPath, path);
            jsonFileName = string.IsNullOrEmpty(jsonFileName) ? Path.Combine(path, "spellcheck.json") : Path.Combine(path, jsonFileName);

            if (File.Exists(jsonFileName))
            {
                string jsonImport = File.ReadAllText(jsonFileName);
                List<DictionaryData> spellChecks = JsonConvert.DeserializeObject<List<DictionaryData>>(jsonImport);
                List<DictionaryData> spellDictCollection = new List<DictionaryData>();
                string personalDictPath = string.Empty;

                foreach (var spellCheck in spellChecks)
                {
                    spellDictCollection.Add(new DictionaryData(spellCheck.LanguadeID, Path.Combine(path, spellCheck.DictionaryPath), Path.Combine(path, spellCheck.AffixPath)));
                    personalDictPath = Path.Combine(path, spellCheck.PersonalDictPath);
                }

                SpellChecker.InitializeDictionaries(spellDictCollection, personalDictPath, cacheCount);
            }

            string licenseKey = string.Empty;

            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "SyncfusionLicense.txt")))
            {
                //Assigning Syncfusion LICENSE_KEY from local file.
                licenseKey = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "SyncfusionLicense.txt")).Trim();
            }
            if (string.IsNullOrEmpty(licenseKey))
            {
                // Access LICENSE_KEY from environment variables
                licenseKey = builder.Configuration["SYNCFUSION_LICENSE_KEY"];
                SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }
            else
            {
                SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                if (HttpMethods.IsOptions(context.Request.Method) && IsDocumentEditorPreflightRequest(context.Request.Path))
                {
                    string requestedOrigin = context.Request.Headers.Origin.ToString();
                    string requestedHeaders = context.Request.Headers.AccessControlRequestHeaders.ToString();
                    string requestedMethod = context.Request.Headers.AccessControlRequestMethod.ToString();

                    context.Response.StatusCode = StatusCodes.Status204NoContent;
                    context.Response.Headers.AccessControlAllowOrigin = string.IsNullOrWhiteSpace(requestedOrigin) ? "*" : requestedOrigin;
                    context.Response.Headers.AccessControlAllowMethods = string.IsNullOrWhiteSpace(requestedMethod) ? "POST, OPTIONS" : requestedMethod + ", OPTIONS";
                    context.Response.Headers.AccessControlAllowHeaders = string.IsNullOrWhiteSpace(requestedHeaders) ? "content-type, authorization, x-requested-with" : requestedHeaders;
                    context.Response.Headers.Vary = "Origin, Access-Control-Request-Method, Access-Control-Request-Headers";
                    return;
                }

                await next();
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowAllOrigin");
            app.UseAuthorization();
            app.UseResponseCompression();

            app.MapControllers();

            app.Run();
        }
        private static bool IsDocumentEditorPreflightRequest(PathString requestPath)
        {
            if (!requestPath.HasValue)
            {
                return false;
            }

            return requestPath.Value!.EndsWith("/api/documenteditor/SpellCheck", StringComparison.OrdinalIgnoreCase)
                || requestPath.Value.EndsWith("/api/documenteditor/SpellCheckByPage", StringComparison.OrdinalIgnoreCase);
        }
    }
}
