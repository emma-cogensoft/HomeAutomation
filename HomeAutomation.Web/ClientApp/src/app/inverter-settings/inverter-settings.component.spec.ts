import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';

import { InverterSettingsComponent } from './inverter-settings.component';

describe('InverterSettingsComponent', () => {
  let fixture: ComponentFixture<InverterSettingsComponent>;
  let httpMock: HttpTestingController;
  const baseUrl = 'https://localhost/';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InverterSettingsComponent],
      providers: [
        { provide: 'BASE_URL', useValue: baseUrl },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting()
      ]
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    fixture = TestBed.createComponent(InverterSettingsComponent);
  });

  afterEach(() => {
    httpMock.match(() => true).forEach(r => r.flush({}));
    httpMock.verify();
  });

  it('should create with loading state before API response', () => {
    expect(fixture.componentInstance).toBeTruthy();
    expect(fixture.componentInstance.inverterData.isLoading).toBeTrue();
    httpMock.expectOne(`${baseUrl}api/invertersettings`).flush({});
  });

  it('should make a GET request to the inverter settings API', () => {
    const req = httpMock.expectOne(`${baseUrl}api/invertersettings`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('should populate data and clear loading flag on successful response', () => {
    const mockData = {
      isLoading: false,
      currentSettingName: 'Winter',
      timeStamp: new Date()
    };

    const req = httpMock.expectOne(`${baseUrl}api/invertersettings`);
    req.flush(mockData);

    expect(fixture.componentInstance.inverterData.currentSettingName).toBe('Winter');
    expect(fixture.componentInstance.inverterData.isLoading).toBeFalse();
  });
});
