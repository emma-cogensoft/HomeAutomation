using HomeAutomation.Application.Services.Weather;
using MediatR;

namespace HomeAutomation.Application.WeatherForecast;

public class GetWeatherForecast : IRequest<Domain.Weather.WeatherForecast>
{
    internal class GetWeatherForecastHandler : IRequestHandler<GetWeatherForecast, Domain.Weather.WeatherForecast>
    {
        private readonly IWeatherForecastReader _weatherForecaster;

        public GetWeatherForecastHandler(IWeatherForecastReader weatherForecaster)
        {
            _weatherForecaster = weatherForecaster;
        }
        
        public async Task<Domain.Weather.WeatherForecast> Handle(GetWeatherForecast request, CancellationToken cancellationToken)
        {
            var weather = await _weatherForecaster.GetForecastForToday(cancellationToken);
            
            return weather;
        }
    }
}
