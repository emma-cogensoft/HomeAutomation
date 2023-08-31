import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-inverter-settings',
  templateUrl: './inverter-settings.component.html'
})
export class InverterSettingsComponent {
  public inverterData: InverterSettings = <InverterSettings>{ isLoading: true };

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<InverterSettings>(baseUrl + 'api/invertersettings').subscribe(result => {
      this.inverterData = result;
      this.inverterData.isLoading = false;
    }, error => console.error(error));
  }
}

interface InverterSettings {
  isLoading: boolean,
  timeStamp: Date,
  currentSettingName: string
}
