import { ChangeDetectorRef, Component, Inject, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  standalone: false,
  selector: 'app-inverter-settings',
  templateUrl: './inverter-settings.component.html'
})
export class InverterSettingsComponent {
  public inverterData: InverterSettings = <InverterSettings>{ isLoading: true };

  constructor(@Inject('BASE_URL') baseUrl: string, private cdr: ChangeDetectorRef) {
    const http = inject(HttpClient);
    http.get<InverterSettings>(baseUrl + 'api/invertersettings').subscribe({
      next: (result: InverterSettings) => {
        this.inverterData = result;
        this.inverterData.isLoading = false;
        this.cdr.markForCheck();
      },
      error: (error: unknown) => console.error(error)
    });
  }
}

interface InverterSettings {
  isLoading: boolean,
  timeStamp: Date,
  currentSettingName: string
}
