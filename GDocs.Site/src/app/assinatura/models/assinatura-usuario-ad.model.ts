export class AssinaturaUsuarioAdModel {
    public notificarFinalizacao: boolean;
    public status: string;
    public descricaoStatus: string;
    public dataAssinatura: Date;
    public PassoId: number;
    public nomeRepresentante: string;
    public justificativa: string;
    public assinarDigitalmente: boolean;
    public assinarFisicamente: boolean;
    public fluxoNotificacao: string;
    constructor(public guid: string, public nome: string, public email: string, public descricao: string, usuarioDeRede: string) { }
}
