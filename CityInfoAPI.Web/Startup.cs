﻿using AspNetCoreRateLimit;
using CityInfoAPI.Data.EF;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Logic.Authentication;
using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                .AddEnvironmentVariables();         // add environ variable to configuration chain
            Configuration = builder.Build();
        }


        // add/register services for our application. add services to container for dependency injection
        public void ConfigureServices(IServiceCollection services)
        {
            string specsName = "CityAPISpecification";
            string aspnetEnvironment = Configuration["ASPNETCORE_ENVIRONMENT"];

            #if DEBUG
                services.AddTransient<IMailService, LocalMailService>();
            #else
                services.AddTransient<IMailService, CloudMailService>();
            #endif

            // the rate limiter AspNetCoreRateLimit package needs to store the counts and rules somewhere.
            // it will use a standard memory cache.
            services.AddMemoryCache();

            // AspNetCoreRateLimit uses IP limiting and client ID limiting. we will use IP limiting.
            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new System.Collections.Generic.List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        // limit any request to any resource to X requests per X minutes/seconds.
                        // only allow 5 requests per 1 minute.
                        Endpoint = "*",
                        Limit = 30,
                        Period = "1m"   // <-- m = minutes, s = seconds
                    },
                    new RateLimitRule()
                    {
                        // we can also add a second rule to only allow x request per x seconds
                        // note that when the API fires up and Swagger kicks in, they each count as 1 request each.
                        Endpoint = "*",
                        Limit = 10,
                        Period = "5s"   // <-- m = minutes, s = seconds
                    }
                };
            });

            // register limiter policy store & rate limit store/
            // they store the policies and rate limit counters.
            // create once and not for each request.
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            services.AddHealthChecks();

            services.AddCors();

            // adding the mvc service
            services.AddMvc(setupAction =>
            {
                // disallows unsupported media type. returns a 406 error
                setupAction.ReturnHttpNotAcceptable = true;

                // allows for xml - both options seem to work although the first seems to be an older method.  add anything other than json.
                //setupAction.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                // allows for xml as an input
                setupAction.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());

                // this will apply an [Authorize] attribute to all controllers
                //setupAction.Filters.Add(new AuthorizeFilter());
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // pick the data source based on the environment
            if (aspnetEnvironment.Equals("local", StringComparison.CurrentCultureIgnoreCase))
            {
                // load the in-memory data store if this is local environment
                services.AddSingleton<ICityInfoRepository, CityInfoMemoryDataStore>();
            }
            else
            {
                // sql data store
                string connectionString = Startup.Configuration["ConnectionStrings:dbConnectionString"];
                services.AddDbContext<CityInfoDbContext>(options => options.UseSqlServer(connectionString));
                services.AddScoped<ICityInfoRepository, CityInfoSqlDataStore>();
            }

            services.AddScoped<CityProcessor>();
            services.AddScoped<CityCollectionsProcessor>();
            services.AddScoped<PointsOfInterestProcessor>();
            services.AddScoped<ReportingProcessor>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // it will look for the letter "v" followed by major and potential minor version
            services.AddVersionedApiExplorer(setupAction =>
            {
                setupAction.GroupNameFormat = "'v'VV";
            });

            // register authentication
            // pass in the default scheme name
            // pass in scheme options (none used here == null)
            // pass in our customer handler as a type
            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", null);

            // register versioning services in our container
            services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            // we need an instance of an ApiVersionDescriptionProvider. we cannot use DI here because we are in the
            // Configure Services provider so we call into BuildServiceProvider on the services object.
            var apiVersionDescriptionProvider = services.BuildServiceProvider().GetService<IApiVersionDescriptionProvider>();

            // accepts a "set up" action (aka: options) to set it up.
            services.AddSwaggerGen(setupAction =>
            {
                // there will be one description doc for each version.
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    // we want to add a swagger document...a specification document.
                    // 1) add name. This will be part of the URI.
                    // 2) add OpenApiInfo object
                    // https://localhost:44305/swagger/OpenAPISpecification/swagger.json

                    // now we can pass the group name in the Swagger doc name.
                    setupAction.SwaggerDoc(
                        $"{specsName}{description.GroupName}",
                        new Microsoft.OpenApi.Models.OpenApiInfo()
                        {
                            Title = "City Info API",
                            Version = description.ApiVersion.ToString(),
                            Description = "City Info DEMO RESTful API",
                            Contact = new Microsoft.OpenApi.Models.OpenApiContact
                            {
                                Email = "shane.fowlkes.70@gmail.com",
                                Name = "D. Shane Fowlkes",
                                Url = new Uri("https://github.com/RedBirdOBX/")
                            },
                            License = new Microsoft.OpenApi.Models.OpenApiLicense
                            {
                                Name = "MIT License",
                                Url = new Uri("https://opensource.org/licenses/mit")
                            }
                        }
                    );
                }

                // lets add the security definitions so (if any) they are added to the documentation.
                // pass it a name ("basicAuth") and a new scheme object
                setupAction.AddSecurityDefinition("basicAuth", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    Description = "Input your user name and password to access this endpoint"
                });

                // let's mark the operation(s) as requiring authentication
                setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "basicAuth"
                            } },
                            new List<string>()
                        }
                });

                // we need to select/find actions to match the versions for the API Explorer
                setupAction.DocInclusionPredicate((documentName, apiDescription) =>
                {
                    var actionApiVersionModel = apiDescription.ActionDescriptor.GetApiVersionModel(ApiVersionMapping.Explicit | ApiVersionMapping.Implicit);

                    if (actionApiVersionModel == null)
                    {
                        return true;
                    }

                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"{specsName}v{v.ToString()}" == documentName);
                    }

                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"{specsName}v{v.ToString()}" == documentName);
                });

                // find the xml comments for the api explorer.  we could do this...
                // setupAction.IncludeXmlComments("CityInfoAPI.Web.xml");

                // ...but since the xml file matches the assembly name, we can use reflection like so:
                //string xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //string fullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                //setupAction.IncludeXmlComments(xmlCommentsFile);

                // since multiple projects will have xml documentation, we will need to loop thru all the files and include
                // all of the xml docs....not just the CityInfoAPI.Web.Xml.
                // **for some reason, these files are not picked up on Azure.**
                DirectoryInfo baseDirectoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                foreach (var fileInfo in baseDirectoryInfo.EnumerateFiles("CityInfoAPI*.xml"))
                {
                    setupAction.IncludeXmlComments(fileInfo.FullName);
                };
            });
        }

        // This method gets called by the runtime.
        // build the request pipeline
        // Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder appBuilder, IHostingEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            string specsName = "CityAPISpecification";

            // add this middleware early so we can reject requests early on.
            appBuilder.UseIpRateLimiting();

            if (env.IsDevelopment())
            {
                appBuilder.UseDeveloperExceptionPage();
            }
            else
            {
                // pass in and return an Action
                appBuilder.UseExceptionHandler(action =>
                {
                    // call run on the action
                    action.Run(async context =>
                    {
                        // set it up so that if exception is hit, it IS a 500 (override it)
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected error has occurred.");
                    });
                });
            }

            appBuilder.UseHttpsRedirection();                  // learn more about this.
            appBuilder.UseHealthChecks("/health");             // learn more about this.
            appBuilder.UseStatusCodePages();
            appBuilder.UseAuthentication();

            // https://stackoverflow.com/questions/44379560/how-to-enable-cors-in-asp-net-core-webapi
            appBuilder.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            appBuilder.UseMvc();

            // EF Mappers
            AutoMapper.Mapper.Initialize(cfg =>
            {
                // Tells AutoMapper to create mappings from Entity (db) to DTOs.
                // <source, destination>
                cfg.CreateMap<CityInfoAPI.Data.Entities.City, CityInfoAPI.Dtos.Models.CityWithoutPointsOfInterestDto>();
                cfg.CreateMap<CityInfoAPI.Data.Entities.City, CityInfoAPI.Dtos.Models.CityDto>();
                cfg.CreateMap<CityInfoAPI.Data.Entities.PointOfInterest, CityInfoAPI.Dtos.Models.PointOfInterestDto>();
                cfg.CreateMap<CityInfoAPI.Dtos.Models.PointOfInterestCreateDto, CityInfoAPI.Data.Entities.PointOfInterest>();
                cfg.CreateMap<CityInfoAPI.Dtos.Models.PointOfInterestCreateRequestDto, CityInfoAPI.Dtos.Models.PointOfInterestCreateDto>();
                cfg.CreateMap<CityInfoAPI.Dtos.Models.PointOfInterestUpdateDto, CityInfoAPI.Data.Entities.PointOfInterest>();
                cfg.CreateMap<CityInfoAPI.Dtos.Models.CityCreateDto, CityInfoAPI.Data.Entities.City>();
                cfg.CreateMap<CityInfoAPI.Dtos.Models.CityUpdateDto, CityInfoAPI.Data.Entities.City>();


                // here's an example of doing a custom member mapping. It will use Projection.
                // It takes CityName from entity and maps it to Name of the DTO.
                //cfg.CreateMap<CityInfoAPI.Data.Entities.City, CityInfoAPI.Dtos.Models.CityDto>()
                //    .ForMember(cityDto => cityDto.Name, option => option.MapFrom(cityEntity => cityEntity.CityName));

                // This backwards mapping is intentional (for Patches).
                // Find the Entity first and map it to a DTO to hide implementation details.
                cfg.CreateMap<CityInfoAPI.Data.Entities.PointOfInterest, CityInfoAPI.Dtos.Models.PointOfInterestUpdateDto>();
                cfg.CreateMap<CityInfoAPI.Data.Entities.City, CityInfoAPI.Dtos.Models.CityUpdateDto>();
            });

            appBuilder.UseSwagger();

            // we need to pass in a setupAction and give it an endpoint where SwaggerUI can find the OpenAPI specs document generated by SwaggerGen.
            // pass in the URI and a name.
            appBuilder.UseSwaggerUI(setupAction =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    setupAction.SwaggerEndpoint($"/swagger/{specsName}{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }

                // helps set up the index.html endpoint
                setupAction.RoutePrefix = string.Empty;

                // shows the method/controller action id in ui. helpful!
                setupAction.DisplayOperationId();

                // shows the time of request to process in ui.
                setupAction.DisplayRequestDuration();
            });
        }
    }

    #pragma warning restore CS1591

}
