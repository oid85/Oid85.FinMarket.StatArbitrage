using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.Repositories;
using Oid85.FinMarket.StatArbitrage.Common.KnownConstants;
using Oid85.FinMarket.StatArbitrage.Infrastructure.Database;
using Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Repositories;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {    
        services.AddDbContextPool<StatArbitrageContext>((serviceProvider, options) =>
        {  
            options.UseNpgsql(configuration.GetValue<string>(KnownSettingsKeys.PostgresStatArbitrageConnectionString)!);
        });

        services.AddPooledDbContextFactory<StatArbitrageContext>(options =>
            options
                .UseNpgsql(configuration.GetValue<string>(KnownSettingsKeys.PostgresStatArbitrageConnectionString)!)
                .EnableServiceProviderCaching(false), poolSize: 32);

        services.AddTransient<IParameterRepository, ParameterRepository>();
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using var scope = scopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<StatArbitrageContext>();
        await context.Database.MigrateAsync();
    }
}