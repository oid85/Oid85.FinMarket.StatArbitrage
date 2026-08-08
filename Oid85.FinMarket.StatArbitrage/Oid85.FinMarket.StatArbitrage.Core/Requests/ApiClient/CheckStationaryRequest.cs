namespace Oid85.FinMarket.StatArbitrage.Core.Requests.ApiClient;

public class CheckStationaryRequest
{
    public List<List<double>> Data { get; set; } = new();
}