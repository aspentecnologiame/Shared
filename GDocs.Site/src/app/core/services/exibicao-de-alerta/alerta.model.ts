import { TipoAlertaEnum } from './tipo-alerta.enum';

export class AlertaModel {
    titulo: string;
    mensagem: string;
    tipo: TipoAlertaEnum;

    textoBotaoOk?: string;
    textoBotaoCancelar?: string;
    exibirBotaoCancelar?: boolean;
    estiloBotaoOk?: string;
}
