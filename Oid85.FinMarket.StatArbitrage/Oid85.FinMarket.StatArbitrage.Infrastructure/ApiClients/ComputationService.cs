using System.Net.Http.Json;
using Oid85.FinMarket.StatArbitrage.Application.Interfaces.ApiClients;
using Oid85.FinMarket.StatArbitrage.Common.KnownConstants;
using Oid85.FinMarket.StatArbitrage.Core.Requests.ApiClient;
using Oid85.FinMarket.StatArbitrage.Core.Responses.ApiClient;

namespace Oid85.FinMarket.StatArbitrage.Infrastructure.ApiClients;

/// <inheritdoc />
public class ComputationApiClient(
    IHttpClientFactory httpClientFactory) 
    : IComputationApiClient
{
    /// <inheritdoc />
    public async Task<List<bool>> CheckStationaryAsync(List<List<double>> data)
    {
        var request = new CheckStationaryRequest { Data = data };
        var response = await GetResponseAsync<CheckStationaryRequest, CheckStationaryResponse>("/api/is-stationary", request);
        return response.Result;
    }

    private async Task<TResponse> GetResponseAsync<TRequest, TResponse>(string url, TRequest request) where TResponse : new()
    {
        try
        {
            var content = JsonContent.Create(request);
            using var httpResponse = await SendPostRequestAsync(url, content);
            var data = await httpResponse.Content.ReadFromJsonAsync<TResponse>();
            return data ?? new TResponse();
        }
        
        catch (Exception exception)
        {
            return new TResponse();
        }
    }
    
    private async Task<HttpResponseMessage> SendPostRequestAsync(string url, HttpContent content)
    {
        using var httpClient = httpClientFactory.CreateClient(KnownHttpClients.FinMarketComputationServiceApiClient);
        return await httpClient.PostAsync(url, content);
    }
}