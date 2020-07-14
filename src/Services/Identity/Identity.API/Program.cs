using Identity.API.Data;
using Identity.API.Infrastructure.Seeds;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Bookcase.Services.Identity.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
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