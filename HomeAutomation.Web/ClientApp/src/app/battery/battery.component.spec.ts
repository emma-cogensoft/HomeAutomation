import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { BatteryComponent } from './battery.component';

describe('BatteryComponent', () => {
  let fixture: ComponentFixture<BatteryComponent>;
  let httpMock: HttpTestingController;
  const baseUrl = 'https://localhost/';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BatteryComponent],
      providers: [
        { provide: 'BASE_URL', useValue: baseUrl },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(BatteryComponent);
  });

  afterEach(() => {
    // Flush any request that was not consumed in this test, then verify no unexpected requests remain
    httpMock.match(() => true).forEach(r => r.flush({}));
    httpMock.verify();
  });

  it('should create with loading state before API response', () => {
    expect(fixture.componentInstance).toBeTruthy();
    expect(fixture.componentInstance.batteryData.isLoading).toBeTrue();
    httpMock.expectOne(`${baseUrl}api/battery`).flush({});
  });

  it('should make a GET request to the battery API', () => {
    const req = httpMock.expectOne(`${baseUrl}api/battery`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('should populate data and clear loading flag on successful response', () => {
    const mockData = {
      isLoading: false,
      totalBatteryCapacityInWh: 5000,
      percentageCharged: 80,
      remainingChargeInWh: 4000,
      remainingBatteryPercentage: 80,
      remainingBatteryCapacityInWh: 4000,
      stateDescription: 'Charging',
      activityDescription: 'Grid charging',
      batteryPowerUsageInW: 1500,
      timeToCompleteInH: 1.5,
      timeStamp: new Date()
    };

    const req = httpMock.expectOne(`${baseUrl}api/battery`);
    req.flush(mockData);

    expect(fixture.componentInstance.batteryData.percentageCharged).toBe(80);
    expect(fixture.componentInstance.batteryData.stateDescription).toBe('Charging');
    expect(fixture.componentInstance.batteryData.isLoading).toBeFalse();
  });
});
