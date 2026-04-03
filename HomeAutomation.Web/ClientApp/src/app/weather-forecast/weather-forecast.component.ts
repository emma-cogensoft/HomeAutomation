import { Component, Inject, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  standalone: false,
  selector: 'app-weather-forecast',
  templateUrl: './weather-forecast.component.html'
})
export class WeatherForecastComponent {
  public weather: WeatherForecast = <WeatherForecast>{ isLoading: true, isSunny: false };

  constructor(@Inject('BASE_URL') baseUrl: string) {
    const http = inject(HttpClient);
    http.get<WeatherForecast>(baseUrl + 'api/forecast').subscribe({
      next: (result: WeatherForecast) => { this.weather = result; },
      error: (error: unknown) => console.error(error)
    });
  }
}

interface WeatherForecast {
  isSunny: boolean,
  isLoading: boolean
}
