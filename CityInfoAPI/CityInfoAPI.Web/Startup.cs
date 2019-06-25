using CityInfoAPI.Data.EF;
using CityInfoAPI.Data.Repositories;
using CityInfoAPI.Logic.Processors;
using CityInfoAPI.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CityInfoAPI.Web
{
    public class Startup
    {


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
            services.AddMvc()
                //.AddJsonOptions(o =>
                //{
                //    if (o.SerializerSettings.ContractResolver != null)
                //    {
                //        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
                //        castedResolver.NamingStrategy = null;
                //    }
                //});
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()));

            // register EF Services
            string connectionString = Startup.Configuration["ConnectionStrings:cityInfoConnectionString"];
            services.AddDbContext<CityInfoDbContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
            services.AddScoped<CityProcessor>();
            services.AddScoped<PointsOfInterestProcessor>();

            // accepts a "set up" action to set it up.
            services.AddSwaggerGen(setupAction =>
            {
                // we want to add a swagger document...a specification document.
                // 1) add name. This will be part of the URI.
                // 2) add OpenApiInfo object
                setupAction.SwaggerDoc(
                    "LibraryOpenAPISpecification",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "City Info API",
                        Version = "1.0"
                    }
                );
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

        }
    }
}
