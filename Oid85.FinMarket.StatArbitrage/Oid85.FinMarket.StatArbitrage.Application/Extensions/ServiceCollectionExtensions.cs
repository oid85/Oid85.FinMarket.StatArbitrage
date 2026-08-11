using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.StatArbitrage.Application.Factories;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Factories;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Application.Services;
using Oid85.FinMarket.StatArbitrage.Application.Strategies;
using Oid85.FinMarket.StatArbitrage.Core.Models;

namespace Oid85.FinMarket.StatArbitrage.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<ICorrelationService, CorrelationService>();
        services.AddScoped<IRegressionTailService, RegressionTailService>();

        services.AddScoped<IMonitorService, MonitorService>();
        services.AddScoped<IDataService, DataService>();
        services.AddScoped<IStatArbitrageService, StatArbitrageService>();

        services.AddScoped<IIndicatorFactory, IndicatorFactory>();

        services.AddKeyedTransient<Strategy, CrossStdDevLongShort>("CrossStdDevLongShort");
        services.AddKeyedTransient<Strategy, CrossStdDevShortLong>("CrossStdDevShortLong");
    }
}