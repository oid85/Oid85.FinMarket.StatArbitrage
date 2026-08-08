namespace Oid85.FinMarket.StatArbitrage.Application.Interfaces.ApiClients;

/// <summary>
/// Работа с сервисом расчетов
/// </summary>
public interface IComputationApiClient
{
    /// <summary>
    /// Выполнить проверку рядов на стационарность
    /// </summary>
    /// <returns></returns>
    Task<List<bool>> CheckStationaryAsync(List<List<double>> data);
}