using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Infrastructure.Seeds
{
    public class ConfigurationDbContextSeed
    {
        public void Seed(ConfigurationDbContext context, IServiceProvider services)
        {
            var cfg = services.GetService<IConfiguration>();
            if (!context.Clients.Any())
            {
                var clientsUrl = new Dictionary<string, string>();
                clientsUrl.Add("CatalogApi", cfg["CatalogApiClient"]);
                clientsUrl.Add("ShelvesApi", cfg["ShelvesApiClient"]);
                foreach (var client in Config.GetClients(clientsUrl))
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var scope in Config.ApiScopes)
                {
                    context.ApiScopes.Add(scope.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResource)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
