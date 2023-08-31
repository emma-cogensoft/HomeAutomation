import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-weather-forecast',
  templateUrl: './weather-forecast.component.html'
})
export class WeatherForecastComponent {
  public weather: WeatherForecast = <WeatherForecast>{ isLoading: true, isSunny: false };

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<WeatherForecast>(baseUrl + 'api/forecast').subscribe(result => {
      this.weather = result;
    }, error => console.error(error));
  }
}

interface WeatherForecast {
  isSunny: boolean,
  isLoading: boolean
}
