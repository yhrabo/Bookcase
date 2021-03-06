﻿using Bookcase.BuildingBlocks.EventBus;
using Bookcase.BuildingBlocks.EventBusRabbitMQ;
using Identity.API.Data;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Reflection;

namespace Bookcase.Services.Identity.API
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddApplicationDbContext(Configuration);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddConfigurationAndOperationStores(Configuration, migrationsAssembly)
                .AddAspNetIdentity<ApplicationUser>();

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();

            services.AddGrpc();
            services.AddEventBasedIntegration(Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<UsersService>();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }

    static class ConfigureExtensionMethods
    {
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(cfg["ConnectionString"],
                providerOptions => providerOptions.EnableRetryOnFailure(
                    cfg.GetValue("SQLDB_MAX_RETRY_COUNT", 2),
                    TimeSpan.FromSeconds(cfg.GetValue("SQLDB_MAX_RETRY_DELAY_IN_SECONDS", 10)),
                    null)));
            return services;
        }

        public static IIdentityServerBuilder AddConfigurationAndOperationStores(this IIdentityServerBuilder builder,
            IConfiguration cfg, string assembly)
        {
            builder
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(
                        cfg["ConnectionString"],
                        sql =>
                        {
                            sql.MigrationsAssembly(assembly);
                            sql.EnableRetryOnFailure(
                                cfg.GetValue("SQLDB_MAX_RETRY_COUNT", 2),
                                TimeSpan.FromSeconds(cfg.GetValue("SQLDB_MAX_RETRY_DELAY_IN_SECONDS", 10)),
                                null);
                        });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder => builder.UseSqlServer(
                        cfg["ConnectionString"],
                        sql =>
                        {
                            sql.MigrationsAssembly(assembly);
                            sql.EnableRetryOnFailure(
                                cfg.GetValue("SQLDB_MAX_RETRY_COUNT", 2),
                                TimeSpan.FromSeconds(cfg.GetValue("SQLDB_MAX_RETRY_DELAY_IN_SECONDS", 10)),
                                null);
                        });
                });
            return builder;
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
            return services;
        }
    }
}