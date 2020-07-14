using Bookcase.Services.Catalog.API.Infrastructure;
using Bookcase.Services.Catalog.API.Infrastructure.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace Bookcase.Services.Catalog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCatalogDbContext(Configuration);
            services.AddSwagger(Configuration);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration["IdentityUrl"];
                options.RequireHttpsMetadata = false;
                options.Audience = "catalog";
            });
        }

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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API V1");
                    c.OAuthClientId("catalogswaggerui");
                    c.OAuthAppName("Catalog Swagger UI");
                });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    static class ConfigureExtensionMethods
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Bookcase - Catalog.API",
                    Description = "Bookcase Catalog Web API"
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
                                { "catalog", "Catalog API" }
                            }
                        }
                    }
                });
                options.OperationFilter<AuthResponsesOperationFilter>();
            });
            return services;
        }

        public static IServiceCollection AddCatalogDbContext(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<CatalogDbContext>(
                options => options.UseSqlServer(cfg["ConnectionString"],
                providerOptions => providerOptions.EnableRetryOnFailure(
                    cfg.GetValue("SQLDB_MAX_RETRY_COUNT", 2),
                    TimeSpan.FromSeconds(cfg.GetValue("SQLDB_MAX_RETRY_DELAY_IN_SECONDS", 20)),
                    null)));
            return services;
        }
    }
}
