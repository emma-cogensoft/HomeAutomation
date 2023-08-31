import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { BatteryComponent } from './battery/battery.component';
import { WeatherForecastComponent } from './weather-forecast/weather-forecast.component';
import {NgOptimizedImage} from "@angular/common";
import {InverterSettingsComponent} from "./inverter-settings/inverter-settings.component";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    BatteryComponent,
    WeatherForecastComponent,
    InverterSettingsComponent
  ],
    imports: [
        BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            {path: '', component: HomeComponent, pathMatch: 'full'},
            {path: 'counter', component: CounterComponent},
            {path: 'battery', component: BatteryComponent},
            {path: 'weather-forecast', component: WeatherForecastComponent},
            {path: 'inverter-settings', component: InverterSettingsComponent},
        ]),
        NgOptimizedImage
    ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
