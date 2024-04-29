using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using HotelBooking.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace HotelBooking.Configurations
{
    public static class MiddlewareExtensions
    {
        public static void UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error($"Something went wrong in the {contextFeature.Error}");

                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal server error. Please try again later"
                        }.ToString()); ;
                    }
                });
            });
        }
    }
}
