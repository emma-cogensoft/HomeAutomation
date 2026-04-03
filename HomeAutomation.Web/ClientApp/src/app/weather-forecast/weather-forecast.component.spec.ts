import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { WeatherForecastComponent } from './weather-forecast.component';

describe('WeatherForecastComponent', () => {
  let fixture: ComponentFixture<WeatherForecastComponent>;
  let httpMock: HttpTestingController;
  const baseUrl = 'https://localhost/';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [WeatherForecastComponent],
      providers: [
        { provide: 'BASE_URL', useValue: baseUrl },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(WeatherForecastComponent);
  });

  afterEach(() => {
    httpMock.match(() => true).forEach(r => r.flush({}));
    httpMock.verify();
  });

  it('should create with loading state before API response', () => {
    expect(fixture.componentInstance).toBeTruthy();
    expect(fixture.componentInstance.weather.isLoading).toBeTrue();
    httpMock.expectOne(`${baseUrl}api/forecast`).flush({});
  });

  it('should make a GET request to the forecast API', () => {
    const req = httpMock.expectOne(`${baseUrl}api/forecast`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('should populate weather data on successful response', () => {
    const mockData = { isSunny: true, isLoading: false };

    const req = httpMock.expectOne(`${baseUrl}api/forecast`);
    req.flush(mockData);

    expect(fixture.componentInstance.weather.isSunny).toBeTrue();
    expect(fixture.componentInstance.weather.isLoading).toBeFalse();
  });
});
