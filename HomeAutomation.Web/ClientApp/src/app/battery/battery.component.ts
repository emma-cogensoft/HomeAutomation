import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-battery',
  templateUrl: './battery.component.html'
})
export class BatteryComponent {
  public batteryData: Battery = <Battery>{ isLoading: true };

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Battery>(baseUrl + 'api/battery').subscribe(result => {
      this.batteryData = result;
      this.batteryData.isLoading = false;
    }, error => console.error(error));
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
  timeToCompleteInH: number
}
