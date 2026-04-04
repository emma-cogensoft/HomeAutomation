import { ChangeDetectorRef, Component, Inject, OnDestroy, OnInit, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { forkJoin } from 'rxjs';

interface BatteryData {
  isLoading: boolean;
  totalBatteryCapacityInWh: number;
  stateDescription: string;
  timeStamp: Date;
  percentageCharged: number;
  remainingChargeInWh: number;
  remainingBatteryPercentage: number;
  remainingBatteryCapacityInWh: number;
  activityDescription: string;
  batteryPowerUsageInW: number;
  timeToCompleteInH: number;
  dataSource: string;
  solarInputInW: number;
  homeUsageInW: number;
  feedInW: number;
}

interface WeatherData {
  isSunny: boolean;
  isLoading: boolean;
}

interface InverterData {
  isLoading: boolean;
  isLoaded: boolean;
  timeStamp: Date;
  currentSettingName: string;
}

interface DashboardState {
  battery: BatteryData;
  weather: WeatherData;
  inverter: InverterData;
  isLoading: boolean;
  lastUpdated: Date | null;
}

@Component({
  standalone: false,
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private readonly http = inject(HttpClient);
  private refreshTimer: ReturnType<typeof setInterval> | null = null;
  private readonly refreshIntervalMs = 60_000;

  state: DashboardState = {
    battery: <BatteryData>{ isLoading: true },
    weather: <WeatherData>{ isLoading: true },
    inverter: <InverterData>{ isLoading: true },
    isLoading: true,
    lastUpdated: null
  };

  constructor(
    @Inject('BASE_URL') private readonly baseUrl: string,
    private readonly cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.refresh();
    this.refreshTimer = setInterval(() => this.refresh(), this.refreshIntervalMs);
  }

  ngOnDestroy(): void {
    if (this.refreshTimer !== null) {
      clearInterval(this.refreshTimer);
    }
  }

  refresh(): void {
    forkJoin({
      battery: this.http.get<BatteryData>(this.baseUrl + 'api/battery'),
      weather: this.http.get<WeatherData>(this.baseUrl + 'api/forecast'),
      inverter: this.http.get<InverterData>(this.baseUrl + 'api/invertersettings')
    }).subscribe({
      next: ({ battery, weather, inverter }) => {
        this.state = {
          battery,
          weather,
          inverter,
          isLoading: false,
          lastUpdated: new Date()
        };
        this.cdr.markForCheck();
      },
      error: () => {
        // Partial failures: try individual calls so one failing API doesn't block the rest
        this.loadIndividually();
      }
    });
  }

  private loadIndividually(): void {
    this.http.get<BatteryData>(this.baseUrl + 'api/battery').subscribe({
      next: battery => { this.state.battery = battery; this.state.isLoading = false; this.state.lastUpdated = new Date(); this.cdr.markForCheck(); },
      error: () => { this.state.battery = <BatteryData>{ isLoading: false }; this.cdr.markForCheck(); }
    });
    this.http.get<WeatherData>(this.baseUrl + 'api/forecast').subscribe({
      next: weather => { this.state.weather = weather; this.cdr.markForCheck(); },
      error: () => { this.state.weather = <WeatherData>{ isLoading: false }; this.cdr.markForCheck(); }
    });
    this.http.get<InverterData>(this.baseUrl + 'api/invertersettings').subscribe({
      next: inverter => { this.state.inverter = inverter; this.cdr.markForCheck(); },
      error: () => { this.state.inverter = <InverterData>{ isLoading: false, currentSettingName: 'Unavailable' }; this.cdr.markForCheck(); }
    });
  }

  get suggestion(): string {
    const battery = this.state.battery;
    const weather = this.state.weather;

    if (!battery || battery.isLoading) return '';

    const pct = battery.percentageCharged;
    const sunny = weather?.isSunny ?? false;
    const charging = battery.activityDescription === 'Charging';

    if (pct >= 95) return '🎉 Battery full! Perfect time to run the washing machine or dishwasher.';
    if (pct < 15) return '⚠️ Battery very low — avoid high-draw appliances like washing machines or tumble dryers.';
    if (sunny && pct >= 80) return '☀️ Sunny and battery nearly full — great time for washing, dishwasher, or charging devices!';
    if (sunny && pct >= 40 && charging) return '☀️ Solar charging in progress — good time to run high-draw appliances.';
    if (sunny && pct < 40) return '☀️ Sunny but battery is low — it\'s charging up. Save heavy loads for later.';
    if (!sunny && pct >= 70) return '🌥️ Plenty of charge stored — use freely.';
    if (!sunny && pct >= 30) return '🌥️ Overcast today — use energy conservatively.';
    if (!sunny && pct < 30) return '🌧️ Battery low and no solar — avoid high-draw appliances.';

    return '✅ System running normally.';
  }

  get powerLabel(): string {
    const battery = this.state.battery;
    if (!battery || battery.isLoading) return '';
    const w = Math.abs(battery.batteryPowerUsageInW);
    const direction = battery.activityDescription === 'Charging' ? '↑' : '↓';
    return `${direction} ${w}W`;
  }

  get isCloudData(): boolean {
    return this.state.battery?.dataSource === 'CloudInverter';
  }

  get inverterAvailable(): boolean {
    return this.state.inverter?.isLoaded === true;
  }
}
