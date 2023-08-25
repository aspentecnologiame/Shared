import { Injectable } from '@angular/core';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { StringHelper } from '../../../helpers/string-helper';
import { AppConfig } from './../../../app.config';

@Injectable({
  providedIn: 'root'
})
export class AppInsightsService {

  loadAppAppInsights(instrumentationKey: string, enableAutoRouteTracking: boolean): ApplicationInsights {
    const appInsights = new ApplicationInsights({
      config: {
        instrumentationKey: instrumentationKey,
        enableAutoRouteTracking: enableAutoRouteTracking
      }
    });
    appInsights.loadAppInsights();
    return appInsights;
  }


  logPageView(name?: string, url?: string) {
    if (!StringHelper.parseToBoolean(AppConfig.settings.appInsights.ativo)) {
      return;
    }

    AppConfig.appInsights.trackPageView({
      name: name,
      uri: url
    });
  }

  logEvent(name: string, properties?: { [key: string]: any }) {
    if (!StringHelper.parseToBoolean(AppConfig.settings.appInsights.ativo)) {
      return;
    }
    AppConfig.appInsights.trackEvent({ name: name }, properties);
  }

  logMetric(name: string, average: number, properties?: { [key: string]: any }) {
    if (!StringHelper.parseToBoolean(AppConfig.settings.appInsights.ativo)) {
      return;
    }
    AppConfig.appInsights.trackMetric({ name: name, average: average }, properties);
  }

  logException(exception: Error, severityLevel?: number) {
    if (!StringHelper.parseToBoolean(AppConfig.settings.appInsights.ativo)) {
      return;
    }
    AppConfig.appInsights.trackException({ exception: exception, severityLevel });
  }

  logTrace(message: string, properties?: { [key: string]: any }) {
    if (!StringHelper.parseToBoolean(AppConfig.settings.appInsights.ativo)) {
      return;
    }
    AppConfig.appInsights.trackTrace({ message: message }, properties);
  }
}
