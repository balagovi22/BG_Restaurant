using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restaurant.Services.Identity.Database;
using Restaurant.Services.Identity.Initializer;
using Restaurant.Services.Identity.Models;
using Restaurant.Services.Identity.Services;

namespace Restaurant.Services.Identity.Extensions
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, 
                                                IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllersWithViews();
            services
                    .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer
                         (configuration.GetConnectionString("IdentityConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryIdentityResources(SD.IdentityResources)
            .AddInMemoryApiScopes(SD.ApiScopes)
            .AddInMemoryClients(SD.Clients)
            .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>();


            builder.AddDeveloperSigningCredential();

            services.AddScoped<IDbInitializer, DbInitializer>();
            

            return services;
        }
    }
}
