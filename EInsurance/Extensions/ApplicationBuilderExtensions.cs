using EInsurance.Data.Seed;

namespace EInsurance.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void SeedDevelopmentData(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>();
        seeder.SeedAsync().GetAwaiter().GetResult();
    }
}
