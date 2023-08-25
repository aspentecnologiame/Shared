import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { StringHelper } from './helpers/string-helper';
import { IAppConfig, IAppServerConfig } from './shared';
import { IConfigApplicationInsights } from './shared/models/app-config-insights.model';
import { AppInsightsService } from './shared/services/app-insights/app-insights.service';


@Injectable()
export class AppConfig {

  static settings: IAppConfig;
  static appInsights: IConfigApplicationInsights;

  constructor(private readonly http: HttpClient) { }

  load() {
    return new Promise((resolve, reject) => {
      const jsonFile = 'assets/config/config.json';
      this.http.get<IAppConfig>(jsonFile)
        .subscribe((response: IAppConfig) => {
          this.http.get<IAppServerConfig>(`${response.apisBaseUrl.gDocs}/v1/configuracao/appsettings.site`)
            .subscribe((responseServer: IAppServerConfig) => {
              const serverConfig = JSON.parse(responseServer.valor);
              AppConfig.settings = Object.assign({}, response, serverConfig);

              if (StringHelper.parseToBoolean(AppConfig.settings.appInsights.ativo)) {
                AppConfig.appInsights = new AppInsightsService().loadAppAppInsights(AppConfig.settings.appInsights.instrumentationKey, StringHelper.parseToBoolean(AppConfig.settings.appInsights.ativo));
              }

              this.http.get<IAppServerConfig>(`${response.apisBaseUrl.gDocs}/v1/configuracao/configCategorias`)
                .subscribe((responseCatServer: IAppServerConfig) => {
                  AppConfig.settings.categorias = JSON.parse(responseCatServer.valor);
                  resolve({});
                },
                  error => {
                    reject(`Could not load url ${response.apisBaseUrl.gDocs}/v1/configuracao/configCategorias: ${JSON.stringify(error)}`);
                  });
            },
              error => {
                reject(`Could not load url ${response.apisBaseUrl.gDocs}/v1/configuracao/appsettings.site: ${JSON.stringify(error)}`);
              });

        },
          error => {
            reject(`Could not load file '${jsonFile}': ${JSON.stringify(error)}`);
          });
    });
  }
}
