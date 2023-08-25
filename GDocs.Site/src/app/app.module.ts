import { CurrencyPipe, DatePipe, registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';
import { APP_INITIALIZER, ErrorHandler, LOCALE_ID, NgModule } from '@angular/core';

import { DateAdapter } from '@angular/material';
import { BrowserModule } from '@angular/platform-browser';
import 'automapper-ts';
import { DateFormat } from '../date-format';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AppConfig } from './app.config';
import { CoreModule } from './core/core.module';
import { HomeLayoutComponent, LoginLayoutComponent } from './layouts';
import { AppInsightsService } from './shared/services/app-insights/app-insights.service';
import { ErrorHandlerService } from './shared/services/app-insights/error-handler.service';

export function initializeApp(appConfig: AppConfig) {
  return () => appConfig.load();
}

registerLocaleData(localePt);

@NgModule({
  declarations: [
    AppComponent,
    HomeLayoutComponent,
    LoginLayoutComponent
  ],
  imports: [
    CoreModule,
    AppRoutingModule,
    BrowserModule
  ],
  providers: [
    DatePipe,
    CurrencyPipe,
    AppConfig,
    ErrorHandlerService,
    AppInsightsService,
    { provide: DateAdapter, useClass: DateFormat },
    { provide: APP_INITIALIZER, useFactory: initializeApp, deps: [AppConfig], multi: true },
    { provide: LOCALE_ID, useValue: 'pt-BR' },
    { provide: ErrorHandler, useClass: ErrorHandlerService }
  ],
  exports: [
    CoreModule,
    BrowserModule
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
  constructor(dateAdapter: DateAdapter<Date>) {
    dateAdapter.setLocale('pt-br'); // DD/MM/YYYY
  }
}
