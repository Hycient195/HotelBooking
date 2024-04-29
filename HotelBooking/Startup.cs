using HotelBooking.Data;
using AspNetCoreRateLimit;
using HotelBooking.Data.Configurations;
using HotelBooking.Core.IReposotory;
using HotelBooking.Core.Repository;
using HotelBooking.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBooking.Configurations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HotelBooking.Core.IReposotory;
using HotelBooking.Core.Repository;
using HotelBooking.Core.Services;
using HealthChecks.UI.Client;


namespace HotelBooking
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // Section of Startup for Adding and Confuguring all Services. This method is executed at runtime
        public void ConfigureServices(IServiceCollection services)
        {

            // Setting Up Context for our database
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("sqlConnection"))
            );

            //services.AddHealthChecks().AddSqlServer(_configuration.GetConnectionString("sqlConnection"));
            services.AddHealthChecks().AddSqlServer("Server=192.168.129.199,1433; database=HotelReservation; User Id=sa; Password=January2024$; TrustServerCertificate=True;");
            services.AddHealthChecksUI().AddInMemoryStorage();


            //services.AddHealthChecksUI();

            /* Sets up memory cache for storing rate limiting data */
            services.AddMemoryCache();

            services.ConfigureRateLimiting(); // Rate limiting

            services.AddHttpContextAccessor(); // Rate limiting

            /* Adding service for response caching */
            services.AddResponseCaching();

            services.ConfigureHttpCacheHeaders();

            /* Configuring Authentication and Identity */
            services.AddAuthentication();

            services.ConfigureIdentity();

            services.ConfigureJWT(_configuration);

            // Adding Cors Policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });


            /* Configuring AutoMapper as a service */
            services.ConfigureAutoMapper();

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //services.AddScoped(typeof(IAuthManager), typeof(AuthManager));
            services.AddScoped<IAuthManager, AuthManager>();


            services.AddSwaggerDoc();


            /*            
                        // Adding of Service for Swagger API Documentation
                        services.AddSwaggerGen(c =>
                        {
                            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotel Booking", Version = "v1", Description = "API for hotel booking application" });
                        });

            */
            // Adding of service for controllers and setting up Newtonsoft to handle JSON serialization
            services.AddControllers(config =>
            {
                config.CacheProfiles.Add("60SecondsDuration", new CacheProfile
                {
                    Duration = 120
                });
            }).AddNewtonsoftJson(option =>
                option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.ConfigureApiVersioning();
        }


        // This section of the startup gets called at rumtime, and is used to middlewares that sit in-between request and response
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Development middleware options
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "HotelBooking v1");
            });

            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");

            app.UseResponseCaching();

            app.UseHttpCacheHeaders();

            app.UseHealthChecks("/healthcheck", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseHealthChecksUI();
            

            app.UseIpRateLimiting(); // Rate Limiting

            app.UseRouting();

            app.UseGlobalExceptionHandling();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }





    ///* Tinkering about with extension methods */
    //public interface IServiceOption
    //{
    //	string DoSomething(string path);
    //};

    //public class ServiceOption : IServiceOption
    //{
    //	public string DoSomething(string path)
    //	{
    //		// Do some weird stuff here with the service object;
    //		return path;
    //	}
    //}

    //public static class MyServiceExtension
    //{
    //       public static IServiceCollection AddCustomContext<T>(this IServiceCollection services, Action<IServiceOption> options)
    //       {
    //		return services.AddCustomContext<T>((options) =>
    //		{
    //			string internalPath = options.DoSomething("");
    //           });

    //	}
    //   }
}

