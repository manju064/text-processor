using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Serilog;
using TextProcessor.Web.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using TextProcessor.Web.Infrastructure.Options;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using TextProcessor.Services.Services;
using TextProcessor.Services.Services.Sort;
using TextProcessor.Services.Services.Statistics;
using TextProcessor.Base.Helpers;
using TextProcessor.Resource;
using TextProcessor.Web.Infrastructure.Filters;

namespace TextProcessor.Web
{
    /// <summary>
    /// Application startup class, configure all components here
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Project startup entry
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="hostingEnvironment"></param>
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        private IConfiguration Configuration { get; }
        private IHostingEnvironment HostingEnvironment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        // ReSharper disable once UnusedMember.Global
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.Formatting = Formatting.Indented;
                });

            services.Configure<AppOptions>(Configuration);

            // Add MVC Core
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            services.AddMvcCore(options =>
                {
                    options.Filters.Add(typeof(ValidateModelStateAttribute));
                })
                .AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV")
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new CustomEnumConverter());
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });


            #region Swagger
            services.AddSwaggerGen(options =>
            {
                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                // integrate xml comments
                options.DescribeAllEnumsAsStrings();
                options.DescribeStringEnumsInCamelCase();
                var docFile = Path.Combine(this.HostingEnvironment.ContentRootPath, "bin", "doc.xml");
                if (File.Exists(docFile))
                    options.IncludeXmlComments(docFile);
            });
            #endregion

            #region API Version
            services.AddApiVersioning(config =>
            {
                config.ReportApiVersions = true;
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.DefaultApiVersion = new ApiVersion(1, 0);
            });
            #endregion

            #region Custom Services
            services.AddScoped<ITextProcessorService, TextProcessorService>();
            services.AddSingleton<IStatisticsHelperFactory, StatisticsHelperFactory>();
            services.AddSingleton(typeof(StringStatisticsHelper));
            services.AddSingleton(typeof(WordStatisticsHelper));
            services.AddSingleton<IComparatorFactory, ComparatorFactory>();
            services.AddSingleton(typeof(AlphanumComparator));
            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="provider"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="appLifetime"></param>
        public void Configure(IApplicationBuilder app,
                              IHostingEnvironment env,
                              IApiVersionDescriptionProvider provider,
                              ILoggerFactory loggerFactory,
                              IApplicationLifetime appLifetime)
        {


            #region logging and exception
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddSerilog();
                // Ensure any buffered events are sent at shutdown
                appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

                loggerFactory.AddFile(Configuration.GetSection("Logging:Serilog"));
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                //TODO, check and see support from azure for logging
                loggerFactory.AddAzureWebAppDiagnostics();
                app.UseExceptionHandler("/Home/Error");
                app.UseResponseCaching();
            }

            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
            }
            #endregion

            #region swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
                c.DefaultModelExpandDepth(2);
                c.DefaultModelRendering(ModelRendering.Model);
                c.DefaultModelsExpandDepth(-1);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.DocExpansion(DocExpansion.None);
                c.EnableDeepLinking();
                c.EnableFilter();
                c.MaxDisplayedTags(5);
                c.ShowExtensions();
            });
            #endregion

            #region Middleware
            // add middleware
            app.UseMiddleware<LogResponseMiddleware>();
            app.UseMiddleware<LogRequestMiddleware>();

            // ErrorWrappingMiddleware should be added after LogResponseMiddleware. Not yet sure why exactly.
            app.UseMiddleware<ErrorWrappingMiddleware>();

            #endregion

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        #region Private methods
        private Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var isDepreciated = description.IsDeprecated ? Global.ApiDepreciated : string.Empty;
            var info = new Info()
            {
                Title = $"{Global.ApiDescription}{isDepreciated}",
                Version = description.ApiVersion.ToString(),
                Description = Global.ApiDescription,
                License = new Swashbuckle.AspNetCore.Swagger.License() { Name = $"© {Global.Author}" }
            };

            return info;
        }
        #endregion
    }
}
