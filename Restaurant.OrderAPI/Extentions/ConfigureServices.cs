using AutoMapper;
using Mango.MessageBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Restaurant.OrderAPI.Database;
using Restaurant.OrderAPI.MappingProfile;
using Restaurant.OrderAPI.Messaging;
using Restaurant.OrderAPI.Repository;

namespace Restaurant.OrderAPI.Extentions
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
                    Title = "Mango.Services.OrderAPI",
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
                        (configuration.GetConnectionString("OrderAPIConnection")));
            services.AddScoped<IOrderRepository, OrderRepository>();
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext> ();
            optionBuilder.UseSqlServer(configuration.GetConnectionString("OrderAPIConnection"));
            services.AddSingleton(new OrderRepository(optionBuilder.Options));
            services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();
            services.AddSingleton<IMessageBus, AzureServiceBusMessageBus>();
        }
    }
}