using NLog;
using ILogger = NLog.ILogger;

namespace Oid85.FinMarket.StatArbitrage.WebHost.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureLogger(this IServiceCollection services)
    {
        LogManager
            .Setup()
            .LoadConfigurationFromFile("nlog.config");

        services.AddTransient(typeof(ILogger), _ => 
            LogManager.GetLogger(AppDomain.CurrentDomain.FriendlyName));
    }

    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.SetIsOriginAllowed(_ => true);
                builder.AllowCredentials();
            });
        });
    }
}