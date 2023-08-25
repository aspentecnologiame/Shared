import { StatusDocumento } from '../enums/status-documento';

export class DocumentoModel {

        public id: number;
        public autorId: string;
        public autor: string;
        public numero: string;
        public descricao: string;
        public referencia: string;
        public dataCriacao: string;
        public tipoPagamento: string;
        public valor: number;
        public quantidadeParcelas?: number;
        public status: StatusDocumento;
        public nomeSalvo: string;
        public descricaoStatus: string;
        public ativo: boolean;
        public moedaSimbolo: string;
        public fornecedor: string;
        public vencimentoPara?: number;
        public prazoEntrega: string;
        public destaque: boolean;
        public referenciaSubstituto: number;
        public binario: string;

}
