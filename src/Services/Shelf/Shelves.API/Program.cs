using Bookcase.Services.Shelves.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Bookcase.Services.Shelves.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.MigrateContext<ShelvesContext>(ctx => new ShelvesContextSeed().Seed(ctx));
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
        public static IHost MigrateContext<TContext>(this IHost host,
            Action<TContext> seeder) where TContext : class
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<TContext>();
                seeder(context);
            }
            return host;
        }
    }
}
