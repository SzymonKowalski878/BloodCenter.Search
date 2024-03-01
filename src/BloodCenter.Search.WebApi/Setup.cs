using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using BloodCenter.Search.Infrastructure;
using Microsoft.Extensions.Options;
using Nest;
using MediatR;
using BloodCenter.Search.Application;
using BloodCenter.Search.Domain.Interfaces;
using Feree.ResultType.Results;
using Newtonsoft.Json;
using BloodCenter.Search.Infrastructure.UserIndex;
using BloodCenter.Search.Domain.Models;
using MassTransit;
using BloodCenter.Search.Application.Events;
using BloodCenter.Identity.Client;

namespace BloodCenter.Search.WebApi
{
    public static class Setup
    {
        public static IConfigurationBuilder SetupConfiguration(
            this IConfigurationBuilder builder,
            ConfigurationManager configManager,
            IWebHostEnvironment environment)
        {
            builder
                .AddConfiguration(configManager)
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            return builder;
        }

        public static void SetupServices(this IServiceCollection services, IConfigurationRoot configuration, bool isIntegrationTests)
        {
            services.AddControllers()
                .AddNewtonsoftJson(c =>
                {
                    var settings = c.SerializerSettings;

                    settings.Converters.Add(new StringEnumConverter());
                    settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    settings.Formatting = Newtonsoft.Json.Formatting.None;
                    settings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                    settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            services.AddSingleton<IConfiguration>(configuration);
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BloodCenter.Search", Version = "v1" });
                c.CustomSchemaIds(type => type.FullName);
            });

            services.AddIdentityClient(configuration);

            services.AddElasticsearch(configuration, isIntegrationTests);
            AddMassTransit(services, isIntegrationTests);
        }

        public static void SetupHost(this ConfigureHostBuilder host, bool isIntegrationTests)
        {
            host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
                var services = new ServiceCollection();
                builder.Populate(services);
                builder.RegisterModule<ApplicationModule>();
                builder.RegisterModule<InfrastructureModule>();

                if (!isIntegrationTests)
                {
                    builder.RegisterBuildCallback(async c =>
                    {
                        var indexes = c.Resolve<IEnumerable<IIndexCreator>>();

                        foreach(var index in indexes)
                        {
                            var result = await index.Create(default);

                            if(result is Failure failure)
                                throw new Exception("Error while creating elastic search index" + JsonConvert.SerializeObject(result));
                        }
                    });
                }
            });

        }

        public static WebApplication SetupApplication(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "BloodCenter.Search v1");
            });

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllers();

            return app;
        }

        private static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration, bool isIntegrationTests)
        {
            services.AddSingleton<IElasticClient>(c =>
            {
                var settings = new ConnectionSettings(new Uri("http://localhost:9200"));

                settings = settings
                        .EnableApiVersioningHeader()
                        .DisableDirectStreaming()
                        .EnableDebugMode()
                        .PrettyJson()
                        .RequestTimeout(new TimeSpan(0, 0, 30))
                        .ServerCertificateValidationCallback((_, _, _, _) => true);

                var client = new ElasticClient(settings);
                return client;
            });


        }

        private static void AddMassTransit(IServiceCollection services, bool isIntegrationTests)
        {
            if (isIntegrationTests)
            {
                services.AddMassTransitTestHarness(busConfigurator =>
                {
                    busConfigurator.AddConsumer<UserAddedEventHandler>();

                    busConfigurator.UsingInMemory((context, cfg) =>
                    {
                        cfg.ConfigureEndpoints(context);
                    });
                });
            }
            else
            {
                services.AddMassTransit(busConfigurator =>
                {
                    busConfigurator.AddConsumer<UserAddedEventHandler>();

                    busConfigurator.UsingRabbitMq((context, cfg) =>
                    {
                        cfg.Host("localhost", "/", h =>
                        {
                            h.Username("guest");
                            h.Password("guest");
                        });

                        cfg.ConfigureEndpoints(context);
                    });
                });

                services.RemoveMassTransitHostedService();
            }
            
        }
    }
}
