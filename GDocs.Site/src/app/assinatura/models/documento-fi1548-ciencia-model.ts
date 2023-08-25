import { CienciaUsuarioAprovacaoModel } from "src/app/saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model";

export class DocumentoFI1548CienciaModel{
    cienciaId: number;
    documentoFI1548Id: number;
    documentoFI1548Numero: number;
    idTipoCiencia: number;
    tipoCiencia: string;
    idStatusCiencia: number;
    statusCiencia: string;
    idUsuario: string;
    nomeUsuario: string;
    observacao: string;
    flgAtivo: boolean;
    ciente: boolean;
    motivoSolicitacao: string;
    justificativa: string;
    fornecedor: string;
    moedaSimbolo: string;
    valorTotal: string;
    pagamento: string;
    motivoRejeicao: string;
    cienciaUsuarioAprovacao: CienciaUsuarioAprovacaoModel[];
}