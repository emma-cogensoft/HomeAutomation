using System.Text.Json;
using System.Text.Json.Nodes;

namespace HomeAutomation.LocalInverter;

public class LocalInverterApiAccessor: ILocalInverterApiAccessor
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LocalInverterApiAccessor(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<JsonObject> GetJsonAsync(Uri uri, string body, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(body);
        
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.PostAsync(uri, new StringContent(body), cancellationToken);
        
        httpResponseMessage.EnsureSuccessStatusCode();
        
        var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(jsonResponse))
        {
            throw new LocalInverterApiException("Could not read response from API");
        }

        return JsonSerializer.Deserialize<JsonObject>(jsonResponse) 
               ?? throw new LocalInverterApiException("Could not deserialize response from API");
    }
    
    public async Task<string> GetStringAsync(Uri uri, string body, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(body);
        
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.PostAsync(uri, new StringContent(body), cancellationToken);
        
        httpResponseMessage.EnsureSuccessStatusCode();
        
        var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(stringResponse))
        {
            throw new LocalInverterApiException("Could not read response from API");
        }

        return stringResponse;
    }
}