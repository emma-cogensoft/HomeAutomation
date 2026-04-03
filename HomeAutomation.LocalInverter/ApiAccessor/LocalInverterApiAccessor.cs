using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HomeAutomation.LocalInverter.ApiAccessor;

public class LocalInverterApiAccessor : ILocalInverterApiAccessor
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

        var httpResponseMessage = await PostAsync(uri, body, cancellationToken);

        JsonObject? jsonResponse;
        try
        {
            jsonResponse = await httpResponseMessage.Content.ReadFromJsonAsync<JsonObject>(cancellationToken: cancellationToken);
        }
        catch (JsonException ex)
        {
            throw new LocalInverterApiException("Could not parse response from local inverter API.", ex);
        }

        return jsonResponse ?? throw new LocalInverterApiException("Local inverter API returned an empty response.");
    }

    public async Task<string> GetStringAsync(Uri uri, string body, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(body);

        var httpResponseMessage = await PostAsync(uri, body, cancellationToken);

        var stringResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(stringResponse))
            throw new LocalInverterApiException("Local inverter API returned an empty response.");

        return stringResponse;
    }

    private async Task<HttpResponseMessage> PostAsync(Uri uri, string body, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient(ServiceCollectionExtensions.HttpClientName);

        HttpResponseMessage httpResponseMessage;
        try
        {
            httpResponseMessage = await httpClient.PostAsync(uri, new StringContent(body), cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new LocalInverterApiException("Request to local inverter API timed out.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new LocalInverterApiException($"Could not connect to local inverter API: {ex.Message}", ex);
        }

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new LocalInverterApiException(
                $"Local inverter API returned {(int)httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}.");
        }

        return httpResponseMessage;
    }
}