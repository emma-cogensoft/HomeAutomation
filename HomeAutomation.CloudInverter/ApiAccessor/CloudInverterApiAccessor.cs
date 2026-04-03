using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HomeAutomation.CloudInverter.ApiAccessor;

public class CloudInverterApiAccessor : ICloudInverterApiAccessor
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

        var httpClient = _httpClientFactory.CreateClient(ServiceCollectionExtensions.HttpClientName);

        HttpResponseMessage httpResponseMessage;
        try
        {
            httpResponseMessage = await httpClient.GetAsync(uri, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new CloudInverterException("Request to cloud inverter API timed out.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new CloudInverterException($"Could not connect to cloud inverter API: {ex.Message}", ex);
        }

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new CloudInverterException(
                $"Cloud inverter API returned {(int)httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}.");
        }

        JsonObject? jsonResponse;
        try
        {
            jsonResponse = await httpResponseMessage.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: cancellationToken);
        }
        catch (JsonException ex)
        {
            throw new CloudInverterException("Could not parse response from cloud inverter API.", ex);
        }

        return jsonResponse ?? throw new CloudInverterException("Cloud inverter API returned an empty response.");
    }
}