using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using log4net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Syncfusion.Dashboard.Core.Helpers;
using Syncfusion.Dashboard.Core.Helpers.Configuration;
using Syncfusion.Dashboard.Designer.Web.Service.Helpers;
using Syncfusion.Dashboard.Service.Base;
using Syncfusion.Dashboard.Service.Base.Cache;
using Syncfusion.Dashboard.Designer.Logger;
using Syncfusion.Dashboard.Service.Base.DashboardServerHelpers;
using IApplicationLifetime = Microsoft.AspNetCore.Hosting.IApplicationLifetime;

namespace Syncfusion.Dashboard.Designer.Web.Service
{
    public class Startup
    {
        private IWebHostEnvironment hostingEnvironment;
        private string applicationPath = "/bi/designer";

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
#if Debug
            var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.cloud.json", optional: true);
            configuration = builder.Build();
#endif
            Configuration = configuration;
            hostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            InitializeLogger();
#if Release || Debug
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
#endif
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                //config.RouteConstraintName = "api-version";
                config.ReportApiVersions = true;
            });
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
            });

            if(Configuration.GetSection("IsAzureApplication").Value=="true")
            {
                var machinekey = Getfiles("config.xml");
                var privatekey = Getfiles("privatekeys.dat");
                AppConfiguration.Initialize(Configuration, machinekey,privatekey, !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BOLD_SERVICES_HOSTING_ENVIRONMENT")) ? Environment.GetEnvironmentVariable("BOLD_SERVICES_HOSTING_ENVIRONMENT"): null);
            }
            else
            AppConfiguration.Initialize(Configuration,null,null, !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BOLD_SERVICES_HOSTING_ENVIRONMENT")) ? Environment.GetEnvironmentVariable("BOLD_SERVICES_HOSTING_ENVIRONMENT") : null);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddLocalizationSupport(new LocalizationOptions
            {
                CaseSensitivity = BoldBI.Localization.LocalizerSourceCultureCaseSensitivity.CaseInsensitive,
                DirectoryPath = AppConfiguration.GetFromAppSettings("locale-path"),
                FileName = AppConfiguration.GetFromAppSettings("locale-file-name"),
            });
            services.AddControllers().AddJsonOptions(jsonOptions => { jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null; });
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed(_ => true)
                    .AllowCredentials()
                    .WithExposedHeaders("ClientId")
                    .SetPreflightMaxAge(TimeSpan.FromMinutes(5));
                });
            });

            services.Configure<HubOptions>(options =>
            {
                options.MaximumReceiveMessageSize = null;
            });
            services.AddAuthorization();
#pragma warning disable CS0612 // Type or member is obsolete
            services.AddSingleton(new PluginLoaderHelper());
#pragma warning restore CS0612 // Type or member is obsolete
            services.AddSignalR(configure =>
            {
                configure.EnableDetailedErrors = true;
                configure.KeepAliveInterval = TimeSpan.FromMinutes(3);
                configure.ClientTimeoutInterval = TimeSpan.FromMinutes(6);
                configure.MaximumReceiveMessageSize = null;
            }).AddJsonProtocol(jsonOptions => { jsonOptions.PayloadSerializerOptions.PropertyNamingPolicy = null; });

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApplicationLifetime applicationLifetime)
        {
            // Handler for application stoppping event.
            applicationLifetime.ApplicationStopping.Register(() =>
            {
                try
                {
                    new CacheManager().WriteColorSetFromCacheOnApplicationEnd();
                }
                catch (Exception ex)
                {
                    BaseLogHandler.LogError($"Error on application end - {ex.Message}", true, ex, System.Reflection.MethodBase.GetCurrentMethod());
                }

            });
#if Release || Debug
            Logger.AppContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
#endif
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(errorObj => errorObj.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature.Error;

                    BaseLogHandler.LogError("Application Error - Server's Last Error", true, exception, MethodBase.GetCurrentMethod());
                }));
            }
            app.Use(async (context, next) =>
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = int.MaxValue;
                // unlimited I guess
                await next.Invoke();
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
            });
            app.UseLocalization();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.Use(async (context, next) =>
            {
                var xproto = context.Request.Headers["X-Forwarded-Proto"].ToString();
                if (xproto != null && xproto.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    context.Request.Scheme = "https";
                }
                var xFor = context.Request.Headers["X-Forwarded-For"].ToString();
                if (xFor != null && !string.IsNullOrWhiteSpace(xFor))
                {
                    IPAddress validIp;
                    var isValidIp = IPAddress.TryParse(xFor, out validIp);
                    if (isValidIp)
                        context.Connection.RemoteIpAddress = validIp;

                }
                await next();
            });
#if (!Release)
            if (!string.IsNullOrWhiteSpace(AppConfiguration.SystemSettings?.InternalAppUrls?.BiDesigner))
            {
                applicationPath = new Uri(AppConfiguration.SystemSettings.InternalAppUrls.BiDesigner).AbsolutePath;
            }
#else
            applicationPath = "/bi/designer";
#endif
            app.UsePathBase(applicationPath);
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Connection")),
                RequestPath = new PathString("/connection")
            });

            app.UseCors();
            app.UseRouting();
            app.UseSession();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            //  app.UseMiddleware<DashboardServiceMiddleWare>();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DesignerHub>("/helper", options =>
                {
                    options.ApplicationMaxBufferSize = 1024000;
                    options.TransportMaxBufferSize = 0;
                });
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health-check");
            });
        }

        public void InitializeLogger()
        {

            var logspath = Configuration.GetSection("AppSettings")["LogsPath"];
            GlobalContext.Properties["loggername"] = Configuration.GetSection("AppSettings")["Loggername"];
            GlobalContext.Properties["AppDataPath"] = !string.IsNullOrEmpty(logspath) ? Path.GetFullPath(logspath, AppDomain.CurrentDomain.BaseDirectory) : AppDomain.CurrentDomain.BaseDirectory;

            //init log4net config
#if Release
            GlobalContext.Properties["IsLogOptionalData"] = Configuration.GetSection("AppSettings")["Log4Net:IsLogOptionalData"];
            var log4netConfig = GetLog4NetConfig();
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["configuration"]["log4net"]);
            BaseLogHandler.SetLogMode("Live");
#else
            XmlDocument log4netConfig = new XmlDocument();
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("BOLD_SERVICES_HOSTING_ENVIRONMENT")) && Environment.GetEnvironmentVariable("BOLD_SERVICES_HOSTING_ENVIRONMENT").Equals("k8s"))
                log4netConfig.Load(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "/logs/k8s/Log4Net.config"));
            else
                log4netConfig.Load(File.OpenRead(AppDomain.CurrentDomain.BaseDirectory + "/logs/Log4Net.config"));
            var repo = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["configuration"]["log4net"]);
            BaseLogHandler.SetLogMode("Development");
#endif
            BaseLogHandler.LogInfo("App started");
        }

        public XmlDocument GetLog4NetConfig()
        {
            XmlDocument log4netConfig = new XmlDocument();
            try
            {
                var log4NetConfileRelativePath = Configuration.GetSection("AppSettings")["Log4Net:ConfigFilePath"];
                Console.WriteLine("Log4nt config relative file path - " + log4NetConfileRelativePath);
                log4NetConfileRelativePath = string.IsNullOrWhiteSpace(log4NetConfileRelativePath) ? "/logs/Log4Net.config" : log4NetConfileRelativePath;
                var log4NetConfigFile = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + log4NetConfileRelativePath);
                Console.WriteLine("Log4nt config file path - " + log4NetConfigFile);
                log4netConfig.Load(log4NetConfigFile);
                var exceptionlessApiKey = Configuration.GetSection("AppSettings")["Log4Net:ExceptionLess:ApiKey"];
                if (!string.IsNullOrWhiteSpace(exceptionlessApiKey))
                {
                    var nodes = log4netConfig.SelectNodes("configuration/log4net/appender/apiKey");
                    foreach (XmlNode node in nodes)
                    {
                        node.Attributes[0].Value = exceptionlessApiKey;
                    }
                }

                var exceptionlessServerUrl = Configuration.GetSection("AppSettings")["Log4Net:ExceptionLess:ServerUrl"];
                if (!string.IsNullOrWhiteSpace(exceptionlessServerUrl))
                {
                    var nodes = log4netConfig.SelectNodes("configuration/log4net/appender/serverUrl");
                    foreach (XmlNode node in nodes)
                    {
                        node.Attributes[0].Value = exceptionlessServerUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while reading log4net config file.");

            }

            return log4netConfig;
        }

        public string Getfiles(string filename)
        {
            
            try
            {
                var connectiontype = Configuration.GetSection("IdP")["AzureBlobConnectionType"];
                var containername = Configuration.GetSection("IdP")["AzureBlobContainerName"];
                var serviceendpoint = Configuration.GetSection("IdP")["AzureBlobServiceEndPoint"];
                var accesskey = Configuration.GetSection("IdP")["AzureBlobStorageAccessKey"];
                var storageURI = Configuration.GetSection("IdP")["AzureBlobStorageUri"];
                var accountname = Configuration.GetSection("IdP")["AzureBlobStorageAccountName"];
                var blobhelper = new AzureBlobHelper(accountname, accesskey, storageURI, connectiontype, containername, string.Empty);

                return blobhelper.DownloadToText(Path.Combine("boldservices/app_data/configuration/", filename));
            }
            catch (Exception ex)
            {
                BaseLogHandler.LogError(ex.Message,true,ex);
                return null;
            }
        }
    }
}
