import { IFeatureCategorias } from "./app-config-feature-categorias.model";
import { IFeatureToggles } from "./app-config-feature-toggles.model";
import { IMoeda } from "./app-config-imoeda.model";

export interface IAppConfig {
  env: {
    name: string;
  };

  appInsights: {
    ativo: string;
    instrumentationKey: string;
    enableAutoRouteTracking: string;
  };

  logging: {
    console: boolean;
  };

  apisBaseUrl: {
    gDocs: string;
  };

  gDocs: {
    Site: {
      ProductVersion: string;
    }
  };

  nomeApp: string;

  loginIntegrado: boolean;

  perfilDefaultId: number;

  fileExtensionsAllowedForUploadToConvertToPdf: string;

  fileExtensionsAllowedForUploadImage: string;

  fileExtensionsAllowedOnlyPdf: string;

  configuracaoAbasPendenciaAssinatura: number[];

  featureToggles: IFeatureToggles[];

  tempoAtualizacaoTelaPendenciaAssinatura: string;

  moedas: IMoeda[];

  statusPagamento: {
    maxColsDescricao: number;
    maxRowsDescricao: number;
  };

  categorias: IFeatureCategorias[];
}
