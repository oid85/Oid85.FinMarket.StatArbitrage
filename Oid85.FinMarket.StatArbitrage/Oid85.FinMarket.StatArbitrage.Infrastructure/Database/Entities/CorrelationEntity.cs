using Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Entities.Base;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.Database.Entities;

public class CorrelationEntity : BaseEntity
{
    public string PortfolioName { get; set; }
    public string TickerFirst { get; set; }
    public string TickerSecond { get; set; }
    public double Value { get; set; }
}