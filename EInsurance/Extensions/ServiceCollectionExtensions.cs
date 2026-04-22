using EInsurance.Data;
using EInsurance.Data.Seed;
using EInsurance.Interfaces;
using EInsurance.Repository;
using EInsurance.Services.Validation;
using Microsoft.EntityFrameworkCore;

namespace EInsurance.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EInsuranceConnection")
            ?? throw new InvalidOperationException("Connection string 'EInsuranceConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<IDevelopmentSeedRepository, DevelopmentSeedRepository>();
        services.AddScoped<DevelopmentDataSeeder>();

        return services;
    }

    public static IServiceCollection AddDataValidationServices(this IServiceCollection services)
    {
        services.AddScoped<IDataValidationService, DataValidationService>();

        return services;
    }
}
