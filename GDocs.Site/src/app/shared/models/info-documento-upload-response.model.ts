import { UploadResponseModel } from "./upload-response.model";

export class InformacoesDocumentoEnviadoUploadResponseModel {
    constructor(
        public enviando: boolean,
        public porcentagemEnviada: number,
        public assinaturaUploadResonse?: UploadResponseModel) { }
}
