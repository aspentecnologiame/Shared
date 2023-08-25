import { UploadResponseModel } from "src/app/shared/models/upload-response.model";

export class AssinaturaPassoItemAssinarRejeitarModel {
  constructor(public listaDeprocessoAssinaturaDocumentoId: number[], public Justificativa: string, public uploadDocumentoComAssinaturaDigital: UploadResponseModel) { }
}
