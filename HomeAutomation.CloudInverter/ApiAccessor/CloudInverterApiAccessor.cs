using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace HomeAutomation.CloudInverter.ApiAccessor;

public class CloudInverterApiAccessor: ICloudInverterApiAccessor
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CloudInverterApiAccessor(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<JsonObject> GetJsonAsync(Uri uri, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(uri);
        
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.GetAsync(uri, cancellationToken);
        
        httpResponseMessage.EnsureSuccessStatusCode();
        
        var jsonResponse = await httpResponseMessage.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: cancellationToken);

        if (jsonResponse == null)
        {
            throw new CloudInverterException("Could not read response from API");
        }

        return jsonResponse;
    }
}