using System.Net.Http.Json;
using System.Text.Json;

namespace HomeAutomation.MetOffice.ApiAccessor;

public class WeatherForecastApiAccessor : IWeatherForecastApiAccessor
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastApiAccessor(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        _httpClientFactory = httpClientFactory;
    }

    public async Task<T> GetAsync<T>(Uri uri, CancellationToken cancellationToken)
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
            throw new MetOfficeApiException("Request to weather API timed out.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new MetOfficeApiException($"Could not connect to weather API: {ex.Message}", ex);
        }

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new MetOfficeApiException(
                $"Weather API returned {(int)httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}.");
        }

        T? jsonResponse;
        try
        {
            jsonResponse = await httpResponseMessage.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
        }
        catch (JsonException ex)
        {
            throw new MetOfficeApiException("Could not parse response from weather API.", ex);
        }

        return jsonResponse ?? throw new MetOfficeApiException("Weather API returned an empty response.");
    }
}