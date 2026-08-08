using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.StatArbitrage.Application.Factories;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Factories;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Application.Services;

namespace Oid85.FinMarket.StatArbitrage.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IStatArbitrageService, StatArbitrageService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IMonitorService, MonitorService>();

        services.AddScoped<IIndicatorFactory, IndicatorFactory>();

        // services.AddKeyedTransient<Strategy, UltimateSmootherInclinationLong>("UltimateSmootherInclinationLong");
    }
}