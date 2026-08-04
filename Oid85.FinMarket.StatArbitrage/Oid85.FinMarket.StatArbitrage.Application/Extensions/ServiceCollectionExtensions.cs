using Microsoft.Extensions.DependencyInjection;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Services;
using Oid85.FinMarket.StatArbitrage.Application.Services;

namespace Oid85.FinMarket.StatArbitrage.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApplicationServices(
        this IServiceCollection services)
    {
        services.AddTransient<IStatArbitrageService, StatArbitrageService>();
    }
}