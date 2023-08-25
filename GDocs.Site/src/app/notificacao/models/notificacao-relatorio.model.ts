
export class NotificacaoRelatorioModel {

    constructor (
        public idNotificacaoUsuario: number, 
        public idRelatorio: number, 
        public idUsuario: string, 
        public lido: boolean,  
        public nome: string, 
        public url: string,
        public parametros: string,
        public rdlBytes: string
        ) 
    { }
}
