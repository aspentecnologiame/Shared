import { AssinaturaInformacoesModel, AssinaturaUploadResponseModel, PassoAssinaturaUsuariosAdAdcionadosPaiModel } from ".";

export class AssinaturaModel {
  constructor(public arquivos: AssinaturaUploadResponseModel[], public informacoes: AssinaturaInformacoesModel, public passos: PassoAssinaturaUsuariosAdAdcionadosPaiModel) { }
}
