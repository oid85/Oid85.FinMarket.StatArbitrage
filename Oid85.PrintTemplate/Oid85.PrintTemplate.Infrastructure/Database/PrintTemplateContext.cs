using Microsoft.EntityFrameworkCore;
using Oid85.PrintTemplate.Common.KnownConstants;
using Oid85.PrintTemplate.Infrastructure.Database.Entities;
using Oid85.PrintTemplate.Infrastructure.Database.Schemas;

namespace Oid85.PrintTemplate.Infrastructure.Database;

public class PrintTemplateContext(DbContextOptions<PrintTemplateContext> options) : DbContext(options)
{
    public DbSet<ParameterEntity> ParameterEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .HasDefaultSchema(KnownDatabaseSchemas.Default)
            .ApplyConfigurationsFromAssembly(
                typeof(PrintTemplateContext).Assembly,
                type => type
                    .GetInterface(typeof(IPrintTemplateSchema).ToString()) != null)
            .UseIdentityAlwaysColumns();
    }    
}