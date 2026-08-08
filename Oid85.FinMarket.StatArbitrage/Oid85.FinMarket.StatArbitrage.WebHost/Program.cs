using System.Text.Json.Serialization;
using Oid85.FinMarket.StatArbitrage.Application.Extensions;
using Oid85.FinMarket.StatArbitrage.Common.Converters;
using Oid85.FinMarket.StatArbitrage.Common.KnownConstants;
using Oid85.FinMarket.StatArbitrage.Core.Configuration;
using Oid85.FinMarket.StatArbitrage.Infrastructure.Extensions;
using Oid85.FinMarket.StatArbitrage.WebHost.Extensions;

namespace Oid85.FinMarket.StatArbitrage.WebHost
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<StatArbitrageSettings>(
                builder.Configuration.GetSection("StatArbitrageSettings")
            );

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                    options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals;
                });

            builder.Services.AddMemoryCache();
            builder.Services.ConfigureLogger();
            builder.Services.ConfigureCors(builder.Configuration);
            builder.Services.ConfigureApplicationServices();
            builder.Services.ConfigureInfrastructure(builder.Configuration);
            builder.Services.ConfigureStorageApiClient(builder.Configuration);
            builder.Services.ConfigureComputationApiClient(builder.Configuration);

            builder.Services.AddWindowsService(options =>
            {
                options.ServiceName = "Oid85.FinMarket.StatArbitrage";
            });

            builder.Services.AddOpenApi();

            bool applyMigrations = builder.Configuration.GetValue<bool>(KnownSettingsKeys.PostgresApplyMigrationsOnStart);
            int port = builder.Configuration.GetValue<int>(KnownSettingsKeys.DeployPort);

            var app = builder.Build();

            if (applyMigrations)
                await app.ApplyMigrations();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });

            app.Urls.Add($"http://0.0.0.0:{port}");

            await app.RunAsync();
        }
    }
}
