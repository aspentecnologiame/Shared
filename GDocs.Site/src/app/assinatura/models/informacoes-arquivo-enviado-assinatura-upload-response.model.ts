import { AssinaturaUploadResponseModel } from './assinatura-upload-response.model';

export class InformacoesDocumentoEnviadoAssinaturaUploadResponseModel {
    constructor(
        public enviando: boolean,
        public porcentagemEnviada: number,
        public assinaturaUploadResonse?: AssinaturaUploadResponseModel) { }
}
