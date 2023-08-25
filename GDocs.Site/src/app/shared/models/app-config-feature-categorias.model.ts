export interface IFeatureCategorias {
  codigo: number;
  numeracaoAutomatica: IFeatureNumeroAutomatico;
  certificadoDigital: IFeaturePadrao;
  assinaturaDocumento: IFeaturePadrao;
  extensoesPermitidas: string;
  templateDir: string;
  flagFareiEnvio:IFeaturePadrao;
}

export interface IFeaturePadrao {
  habilitado: boolean;
  mustache: string;
}

export interface IFeatureNumeroAutomatico extends IFeaturePadrao {
  chaveSequence: string;
}
