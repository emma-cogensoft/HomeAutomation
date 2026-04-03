import { Component, Inject, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  standalone: false,
  selector: 'app-battery',
  templateUrl: './battery.component.html'
})
export class BatteryComponent {
  public batteryData: Battery = <Battery>{ isLoading: true };

  constructor(@Inject('BASE_URL') baseUrl: string) {
    const http = inject(HttpClient);
    http.get<Battery>(baseUrl + 'api/battery').subscribe({
      next: (result: Battery) => {
        this.batteryData = result;
        this.batteryData.isLoading = false;
      },
      error: (error: unknown) => console.error(error)
    });
  }
}

interface Battery {
  isLoading: boolean,
  totalBatteryCapacityInWh: number,
  stateDescription: string,
  timeStamp: Date,
  percentageCharged: number,
  remainingChargeInWh: number,
  remainingBatteryPercentage: number,
  remainingBatteryCapacityInWh: number,
  activityDescription: string,
  batteryPowerUsageInW: number,
  timeToCompleteInH: number,
  dataSource: string
}
