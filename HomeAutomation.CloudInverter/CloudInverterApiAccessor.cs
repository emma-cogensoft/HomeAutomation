using System.Text.Json;
using System.Text.Json.Nodes;

namespace HomeAutomation.CloudInverter;

public class CloudInverterApiAccessor: ICloudInverterApiAccessor
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CloudInverterApiAccessor(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<JsonObject> ReadAsync(Uri uri, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(uri);
        
        var httpClient = _httpClientFactory.CreateClient();
        var httpResponseMessage = await httpClient.GetAsync(uri, cancellationToken);
        
        httpResponseMessage.EnsureSuccessStatusCode();
        
        var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(jsonResponse))
        {
            throw new CloudInverterException("Could not read response from API");
        }

        return JsonSerializer.Deserialize<JsonObject>(jsonResponse) 
               ?? throw new CloudInverterException("Could not deserialize response from API");
    }
}