using HotelBooking;
using HotelBooking.Data;
using HotelBooking.Core.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;

//var builder = WebApplication.CreateBuilder(args);

//var app = builder.Build();



static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });



Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "./logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{level:u3}] {message:lj}{Newline}{Exception}",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information
    ).CreateLogger();


try
{
    CreateHostBuilder(args).Build().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}


//app.MapGet("/", (HttpContext context) =>
//{
//    //return context.Request.Headers["User-Agent"];
//    context.Response.Headers.ContentType = "text/html";
//    return @"<div>
//        <button onclick=""clickButton()"">Click Me</button>
//        <script>
//            function clickButton(){
//                alert(""Welcome to minimal APIs"");
//            }
//        </script>
//    </div>";
//});

//app.MapGet("/products", (HttpContext context) =>
//{
//    return "ID is " + context.Request.Query["id"] + " category is " + context.Request.Query["category"];
//});

//app.Run(async (HttpContext context) =>
//{
//    //context.Response.StatusCode = 200;
//    await context.Response.WriteAsync("Catch all route in it and");
//    //return;
//});

//app.Run();