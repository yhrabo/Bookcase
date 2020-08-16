using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using WebMVC.Areas.Catalog.Services;
using WebMVC.Areas.Shelves.Infrastructure;
using WebMVC.Areas.Shelves.Services;
using WebMVC.Infrastructure;
using WebMVC.Services;

namespace WebMVC
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
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            services.AddHttpClientServices();
            services.AddSession();
            services.Configure<AppSettings>(Configuration);
            services.AddControllers();
            services.AddCustomAuthentication(Configuration);
            services.AddJsonSerializerOptions();
            services.AddTransient<IUsersService, GrpcUsersService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "areaCatalog",
                    areaName: "Catalog",
                    pattern: "Catalog/{controller=Authors}/{action=Index}/{id?}"
                    );
                endpoints.MapAreaControllerRoute(
                    name: "areaShelves",
                    areaName: "Shelves",
                    pattern: "{controller=Shelves}/{action}/{shelfId?}/{bookId?}"
                    );
                endpoints.MapControllerRoute(
                    name: "Controller",
                    pattern: "{controller}/{action}"
                    );
                endpoints.MapControllers();
                //endpoints.MapGet("/routes", request =>
                //{
                //    request.Response.Headers.Add("content-type", "application/json");
                //    var ep = endpoints.DataSources.First().Endpoints.Select(e => e as RouteEndpoint);
                //    return request.Response.WriteAsync(
                //        JsonSerializer.Serialize(
                //            ep.Select(e => new
                //            {
                //                Method = ((HttpMethodMetadata)e.Metadata.FirstOrDefault(m => m.GetType() == typeof(HttpMethodMetadata)))?.HttpMethods.First(),
                //                Route = e.RoutePattern.RawText
                //            }),
                //            new JsonSerializerOptions
                //            {
                //                WriteIndented = true
                //            }
                //        )
                //        );
                //});
            });
        }
    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(cfg.GetValue("SessionCookieLifetimeMinutes", 120));
            })
            .AddOpenIdConnect(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = cfg.GetValue<string>("IdentityUrl");
                options.SignedOutRedirectUri = cfg.GetValue<string>("CallbackUrl");
                //options.SignedOutCallbackPath = "/Catalog";
                options.ClientId = "mvc";
                options.ClientSecret = "secret";
                options.ResponseType = "code id_token";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("catalog");
                options.Scope.Add("shelves");
            });

            return services;
        }

        public static IServiceCollection AddHttpClientServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IAuthorService, HttpAuthorService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();
            services.AddHttpClient<IShelfService, HttpShelfService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

            return services;
        }

        public static IServiceCollection AddJsonSerializerOptions(this IServiceCollection services)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            options.Converters.Add(new AccessLevelJsonConverter());
            services.AddSingleton(options);
            return services;
        }
    }
}
