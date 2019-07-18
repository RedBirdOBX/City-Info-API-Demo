using CityInfoAPI.Data.EF;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CityInfoAPI.Web
{

    #pragma warning disable CS1591

    public class Startup
    {
        // properties
        public static IConfiguration Configuration;

        // constructor
        // we will need to inject the hosting environment.  we need this to point the framework to the root of our application.
        public Startup(IHostingEnvironment environment)
        {
            // instantiate a new configuration builder used to create a configuration instance which we will need.
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile($"appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(); // add environ variable to configuration chain
            Configuration = builder.Build();
        }


        // add/register services for our application.
        // add services to container for dependency injection
        public void ConfigureServices(IServiceCollection services)
        {

            #if DEBUG
                services.AddTransient<IMailService, LocalMailService>();
            #else
                services.AddTransient<IMailService, CloudMailService>();
            #endif

            // adding the mvc service
            services.AddMvc(setupAction =>
            {
                // disallows unsupported media type. returns a 406 error
                setupAction.ReturnHttpNotAcceptable = true;

                // allows for xml
                setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            });


            //.AddJsonOptions(o =>
            //{
            //    if (o.SerializerSettings.ContractResolver != null)
            //    {
            //        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //        castedResolver.NamingStrategy = null;
            //    }
            //});


            // register EF Services
            string connectionString = Startup.Configuration["ConnectionStrings:cityInfoConnectionString"];
            services.AddDbContext<CityInfoDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
            services.AddScoped<CityProcessor>();
            services.AddScoped<PointsOfInterestProcessor>();

            //services.AddAutoMapper();

            // register versioning services in our container
            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            // accepts a "set up" action to set it up.
            services.AddSwaggerGen(setupAction =>
            {
                // we want to add a swagger document...a specification document.
                // 1) add name. This will be part of the URI.
                // 2) add OpenApiInfo object
                // https://localhost:44305/swagger/OpenAPISpecification/swagger.json
                setupAction.SwaggerDoc(
                    "OpenAPISpecification",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "City Info API",
                        Version = "1.0",
                        Description = "City Info DEMO RESTful API",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact
                        {
                            Email = "shane.fowlkes.70@gmail.com",
                            Name = "D. Shane Fowlkes",
                            Url = new Uri("https://github.com/RedBirdOBX/")
                        },

                        // you can also include licensing information
                        License = new Microsoft.OpenApi.Models.OpenApiLicense
                        {
                            Name = "MIT License",
                            Url = new Uri("https://opensource.org/licenses/mit")
                        }
                    }
                );

                // we could do this...
                // setupAction.IncludeXmlComments("CityInfoAPI.Web.xml");

                // but since the xml file matches the assembly name, we can use reflection like so:
                //string xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //string fullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                //setupAction.IncludeXmlComments(xmlCommentsFile);

                // since multiple projects will have xml documentation, we will need to loop thru all the files and include
                // all of the xml docs....not just the CityInfoAPI.Web.Xml.
                DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                foreach (var fileInfo in baseDirectoryInfo.EnumerateFiles("CityInfoAPI*.xml"))
                {
                    setupAction.IncludeXmlComments(fileInfo.FullName);
                };
            });

        }

        // This method gets called by the runtime.
        // build pipeline
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CityInfoDbContext cityInfoDbContext)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug(LogLevel.Information);
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // problem here
                //app.UseExceptionHandler();
            }

            app.UseHttpsRedirection();      // <-- learn more about this.
            app.UseStatusCodePages();
            app.UseMvc();

            // EF Mappers
            AutoMapper.Mapper.Initialize(cfg =>
            {
                // Tells AutoMapper to create mappings from Entity (db) to DTOs.
                // <source, destination>
                cfg.CreateMap<CityInfoAPI.Data.Entities.City, CityInfoAPI.Dtos.Models.CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<CityInfoAPI.Data.Entities.City, CityInfoAPI.Dtos.Models.CityDto>();
                cfg.CreateMap<CityInfoAPI.Data.Entities.PointOfInterest, CityInfoAPI.Dtos.Models.PointOfInterestDto>();
                cfg.CreateMap<CityInfoAPI.Dtos.Models.PointOfInterestCreateDto, CityInfoAPI.Data.Entities.PointOfInterest>();
                cfg.CreateMap<CityInfoAPI.Dtos.Models.PointOfInterestUpdateDto, CityInfoAPI.Data.Entities.PointOfInterest>();

                // This backwards mapping is intentional (for Patches).
                // Find the Entity first and map it to a DTO to hide implementation details.
                cfg.CreateMap<CityInfoAPI.Data.Entities.PointOfInterest, CityInfoAPI.Dtos.Models.PointOfInterestUpdateDto>();
            });

            app.UseSwagger();

            // We need to pass in a setupAction and give it an endpoint where SwaggerUI
            // can find the OpenAPI specs document generated by SwaggerGen.
            // pass in the URI and a name
            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/OpenAPISpecification/swagger.json", "City Info API");

                // helps set up the index.html endpoint
                setupAction.RoutePrefix = string.Empty;
            });
        }
    }

    #pragma warning restore CS1591

}
