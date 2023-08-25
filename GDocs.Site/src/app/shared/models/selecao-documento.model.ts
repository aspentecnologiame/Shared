import { AssinaturaUploadResponseModel } from "src/app/assinatura/models/assinatura-upload-response.model";

export class SelecaoDocumentoModel {
    constructor(
        public numeroDocumentoSelecionado: number,
        public arquivos: AssinaturaUploadResponseModel[]) { }
}
