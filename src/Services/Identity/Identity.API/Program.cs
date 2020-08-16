using Identity.API.Data;
using Identity.API.Infrastructure.Seeds;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Net;

namespace Bookcase.Services.Identity.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var cfg = GetConfiguration();
            var host = CreateHostBuilder(cfg, args).Build();
            host.MigrateDbContext<ConfigurationDbContext>((context, services) =>
            {
                new ConfigurationDbContextSeed()
                .Seed(context, services);
            });
            host.MigrateDbContext<PersistedGrantDbContext>((context, services) => { });
            host.MigrateDbContext<ApplicationDbContext>((context, services) =>
            {
                new ApplicationDbContextSeed()
                .Seed(context, services);
            });
            host.Run();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            return builder.Build();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration cfg, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        var ports = GetDefinedPorts(cfg);
                        options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                        options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });

        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration cfg)
        {
            var grpcPort = cfg.GetValue("GRPC_PORT", 81);
            var port = cfg.GetValue("PORT", 80);
            return (port, grpcPort);
        }
    }

    static class HostExtensions
    {
        public static IHost MigrateDbContext<TContext>(this IHost host,
            Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<TContext>();
                context.Database.Migrate();
                seeder(context, services);
            }
            return host;
        }
    }
}