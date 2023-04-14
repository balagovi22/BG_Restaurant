using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Restaurant.ProductAPI.Database;
using Restaurant.ProductAPI.MappingProfile;
using Restaurant.ProductAPI.Repository;

namespace Restaurant.ProductAPI.Extentions
{
    public static class ConfigureServices
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services,
                                              IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            ConfigureSwagger(services);
            ConfigureDatabase(services, configuration);
            ConfigureAutoMapper(services);
            ConfigureDependency(services);
            ConfigureAuthentication(services);
            ConfigureAuthorization(services);
            return services;
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Mango.Services.ProductAPI",
                });
                c.EnableAnnotations();
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = @"Enter 'Bearer' [space] and your token",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                },
                                Scheme="oauth2",
                                Name="Bearer",
                                In=ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });
            });
        }

        private static void ConfigureAuthentication(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://localhost:7131";
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });
        }

        private static void ConfigureAuthorization(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "mango");
                });
            });
        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            IMapper mapper = MappingProfileConfig.RegisterMaps().CreateMapper();
            services.AddSingleton(mapper);
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
        {
            services
            .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer
                        (configuration.GetConnectionString("ProductConnection")));
        }

        private static void ConfigureDependency(IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
        }
    }
}