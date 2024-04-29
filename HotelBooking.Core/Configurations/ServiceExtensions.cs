using AspNetCoreRateLimit;
using HotelBooking.Configurations;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace HotelBooking.Data.Configurations
{
    public static class ServiceExtensions
    {
        /* Configuration for AutoMapper */
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            //services.AddAutoMapper(typeof(MapperInitializer)); // This method is suitable for when there's only one MapperInitializer file
            services.AddAutoMapper(Assembly.GetExecutingAssembly()); // This method is suitable for when there are several MapperInitializer files
        }

        /* Configuration for Identity Core */
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);

            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }

        /* Service Extension for Setting Up JWT Authentication */
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            //var key = Environment.GetEnvironmentVariable("KEY");

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ffc632ce-0053-4bab-8077-93a4d14caaad")),
                    ValidateAudience = false
                };
            });
        }

        /* Service Extension for Setting Up Swagger Documentation */
        public static void AddSwaggerDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(config =>
            {
                config.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme.
                        Use the prefix 'Bearer', a space and then your token as seen below
                        Example: 'Bearer ey4safd8ias'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "0auth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Hotel Booking",
                    Version = "v1",
                    Description = "API for hotel booking application"
                });
            });
        }

        public static void ConfigureApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);

                // option.ApiVersionReader = new HeaderApiVersionReader("api-version"); // This option allows you to specify the api version to be used as a header key-value pair
            });
            /*It is good to note that the 3 kinds of API vesion specification cannot be used
             all at once. only one must be used at a time.
             The 3 versioning approaches include
                 1. Included in the API rout path
                 2. Included as a route query parameter
                 3. Included as a header key-value pair.
            */
        }


        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddHttpCacheHeaders(
                (expirationOption) =>
                {
                    expirationOption.MaxAge = 130;
                    expirationOption.CacheLocation = CacheLocation.Private;
                },
                (validationOption) =>
                {
                    validationOption.MustRevalidate = true;
                }
            );
        }

        /* Configuring Throttling/Rate Limiting */
        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            services.Configure<IpRateLimitOptions>(option =>
            {
                option.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Limit = 1,
                        Period = "5s"
                    }
                };
            });


            // Other necessary add-ond required for rate-limiting to work
            services.AddInMemoryRateLimiting();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}
