using Microsoft.EntityFrameworkCore;
using Oid85.FinMarket.StatArbitrage.Common.KnownConstants;
using Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Entities;
using Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Schemas;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Database;

public class StatArbitrageContext(DbContextOptions<StatArbitrageContext> options) : DbContext(options)
{
    public DbSet<StrategyExecuteResultEntity> StrategyExecuteResultEntities { get; set; }
    public DbSet<ParameterEntity> ParameterEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .HasDefaultSchema(KnownDatabaseSchemas.Default)
            .ApplyConfigurationsFromAssembly(
                typeof(StatArbitrageContext).Assembly,
                type => type
                    .GetInterface(typeof(IStatArbitrageSchema).ToString()) != null)
            .UseIdentityAlwaysColumns();
    }    
}