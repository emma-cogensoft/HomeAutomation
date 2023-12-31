using System.Net.Http.Json;

namespace HomeAutomation.MetOffice.ApiAccessor;

public class WeatherForecastApiAccessor: IWeatherForecastApiAccessor
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
        
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var httpResponseMessage = await httpClient.GetAsync(uri, cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();
        
        var jsonResponse = await httpResponseMessage.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);

        if (jsonResponse == null)
        {
            throw new MetOfficeApiException("Could not read response from API");
        }

        return jsonResponse;
    }
}