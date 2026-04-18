using EInsurance.Extensions;
using EInsurance.Middleware;
using Serilog;

namespace EInsurance;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
           .WriteTo.Console()
           .WriteTo.File("logs/log_.txt", rollingInterval: RollingInterval.Day)
           .CreateLogger();

        builder.Host.UseSerilog();



        builder.Services.AddControllersWithViews();
        builder.Services.AddApplicationDataAccess(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddApplicationServices();

        var app = builder.Build();

        // Only seed development data in Development environment
        if (app.Environment.IsDevelopment())
        {
            app.SeedDevelopmentData();
        }

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Add rate limiting middleware for authentication endpoints
        app.UseMiddleware<RateLimitingMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");

        app.Run();
    }
}
