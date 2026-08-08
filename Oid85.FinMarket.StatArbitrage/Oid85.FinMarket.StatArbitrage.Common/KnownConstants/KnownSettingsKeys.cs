namespace Oid85.FinMarket.StatArbitrage.Common.KnownConstants;

public static class KnownSettingsKeys
{
    public const string PostgresStatArbitrageConnectionString = "Postgres:StatArbitrageConnectionString";
    public const string PostgresApplyMigrationsOnStart = "Postgres:ApplyMigrationsOnStart";
    public const string DeployPort = "DeployPort";
    public const string FinMarketStorageServiceApiClientBaseAddress = "FinMarketStorageServiceApiClient:BaseAddress";
    public const string FinMarketComputationServiceApiClientBaseAddress = "FinMarketComputationServiceApiClient:BaseAddress";
}