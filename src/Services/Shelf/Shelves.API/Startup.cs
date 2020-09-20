using Bookcase.BuildingBlocks.EventBus;
using Bookcase.BuildingBlocks.EventBusRabbitMQ;
using Bookcase.Services.Shelves.API.Infrastructure;
using Bookcase.Services.Shelves.API.IntegrationEvents;
using Bookcase.Services.Shelves.API.Models;
using Bookcase.Services.Shelves.API.Services;
using Bookcase.Services.Shelves.API.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RabbitMQ.Client;
using Shelves.API.Infrastructure.Middleware;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bookcase.Services.Shelves.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(cfg =>
                    cfg.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.Configure<ShelvesSettings>(Configuration);
            services.AddTransient<IShelfService, ShelfService>();
            services.AddClassMapForShelvesContext();
            services.AddSingleton<ShelvesContext>();
            services.AddSwagger(Configuration);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration["IdentityUrl"];
                options.RequireHttpsMetadata = false;
                options.Audience = "shelves";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("EditPolicy", policy =>
                    policy.Requirements.Add(new IsOwnerRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, ShelfOwnerAuthorizationHandler>();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("ShelvesApiScope", policy =>
            //    {
            //        policy.RequireAuthenticatedUser();
            //        policy.RequireClaim("scope", "shelves");
            //    });
            //});
            services.AddEventBasedIntegration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shelves.API V1");
                    c.OAuthClientId("shelvesswaggerui");
                    c.OAuthAppName("Shelves Swagger UI");
                });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //.RequireAuthorization("ShelvesApiScope");
            });

            app.SubscribeToEvents();
        }
    }

    static class IAppBuilderExtensions
    {
        public static IApplicationBuilder SubscribeToEvents(this IApplicationBuilder appBuilder)
        {
            var eventBus = appBuilder.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>();
            return appBuilder;
        }
    }

    static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers class to database collection mapping for MongoDB.
        /// </summary>
        /// <param name="services"></param>
        /// <returns>A reference to this instance after the operation is completed.</returns>
        public static IServiceCollection AddClassMapForShelvesContext(this IServiceCollection services)
        {
            BsonClassMap.RegisterClassMap<ShelfViewModel>(cm =>
            {
                cm.AutoMap();
                cm.IdMemberMap.SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
            });
            BsonClassMap.RegisterClassMap<ShelvesViewModel>(cm =>
            {
                cm.AutoMap();
                cm.IdMemberMap.SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
            });
            BsonClassMap.RegisterClassMap<Shelf>(cm =>
            {
                cm.AutoMap();
                cm.IdMemberMap.SetSerializer(new StringSerializer(MongoDB.Bson.BsonType.ObjectId));
            });
            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services,
            IConfiguration cfg)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Bookcase - Shelves.API",
                    Description = "Bookcase Shelves Web API"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{cfg.GetValue<string>("IdentityUrl")}/connect/authorize"),
                            TokenUrl = new Uri($"{cfg.GetValue<string>("IdentityUrl")}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                { "shelves", "Shelves API" }
                            }
                        }
                    }
                });
                options.OperationFilter<AuthResponsesOperationFilter>();
            });
            return services;
        }

        public static IServiceCollection AddEventBasedIntegration(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddSingleton<IEventBusSubscriptionManager, InMemoryEventBusSubscriptionManager>();
            services.AddSingleton<IPersistentConnection>(services =>
            {
                var logger = services.GetRequiredService<ILogger<DefaultPersistentConnection>>();
                var factory = new ConnectionFactory()
                {
                    HostName = cfg.GetValue<string>("EventBusHostName"),
                    UserName = cfg.GetValue("EventBusUserName", string.Empty),
                    Password = cfg.GetValue("EventBusPassword", string.Empty),
                    DispatchConsumersAsync = true,
                };
                return new DefaultPersistentConnection(factory, logger);
            });
            services.AddSingleton<IEventBus>(services =>
            {
                var subsManager = services.GetRequiredService<IEventBusSubscriptionManager>();
                var persConn = services.GetRequiredService<IPersistentConnection>();
                var exName = cfg.GetValue<string>("MessageBroker:ExchangeName");
                var quName = cfg.GetValue<string>("MessageBroker:QueueName");
                var logger = services.GetRequiredService<ILogger<RabbitMQEventBus>>();
                return new RabbitMQEventBus(subsManager, persConn, exName, quName, logger, services);
            });

            services.AddTransient<UserCreatedIntegrationEventHandler>();

            return services;
        }
    }
}
