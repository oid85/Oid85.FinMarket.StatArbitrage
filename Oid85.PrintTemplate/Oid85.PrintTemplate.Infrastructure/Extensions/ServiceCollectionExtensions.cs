using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oid85.PrintTemplate.Application.Interfaces.Repositories;
using Oid85.PrintTemplate.Common.KnownConstants;
using Oid85.PrintTemplate.Infrastructure.Database;
using Oid85.PrintTemplate.Infrastructure.Database.Repositories;

namespace Oid85.PrintTemplate.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {    
        services.AddDbContextPool<PrintTemplateContext>((serviceProvider, options) =>
        {  
            options.UseNpgsql(configuration.GetValue<string>(KnownSettingsKeys.PostgresPrintTemplateConnectionString)!);
        });

        services.AddPooledDbContextFactory<PrintTemplateContext>(options =>
            options
                .UseNpgsql(configuration.GetValue<string>(KnownSettingsKeys.PostgresPrintTemplateConnectionString)!)
                .EnableServiceProviderCaching(false), poolSize: 32);

        services.AddTransient<IParameterRepository, ParameterRepository>();
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        var scopeFactory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using var scope = scopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<PrintTemplateContext>();
        await context.Database.MigrateAsync();
    }
}