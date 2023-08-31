namespace HomeAutomation.WeatherForecastMetOffice.HttpAccessor;

public class WeatherForecastApiAccessor: IWeatherForecastApiAccessor
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastApiAccessor(IHttpClientFactory httpClientFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<string> GetAsJsonStringAsync(Uri uri, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(uri);
        
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        
        var httpResponseMessage = await httpClient.GetAsync(uri, cancellationToken);

        httpResponseMessage.EnsureSuccessStatusCode();
        
        var jsonResponse = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(jsonResponse))
        {
            throw new WeatherForecastMetOfficeApiException("Could not read response from API");
        }

        return jsonResponse;
    }
}