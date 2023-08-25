import { Injectable } from '@angular/core';
import 'automapper-ts';
import swal, { SweetAlertOptions, SweetAlertResult } from 'sweetalert2';
import { AlertaModel } from './alerta.model';
import { TipoAlertaEnum } from './tipo-alerta.enum';

@Injectable({
  providedIn: 'root'
})
export class ExibicaoDeAlertaService {

  private swalWithBootstrapButtons(options?: SweetAlertOptions): Promise<SweetAlertResult> {
    const optionsDefault = {
      confirmButtonClass: 'btn btn-primary',
      cancelButtonClass: 'btn btn-danger',
      buttonsStyling: false
    };

    return swal(this.assignDefined(optionsDefault, options));
  }

  exibirMensagem(alerta: AlertaModel): Promise<SweetAlertResult> {
    const alertPlugin = automapper.map('AlertaModel', 'SweetAlertOptions', alerta);
    return this.swalWithBootstrapButtons(alertPlugin);
  }

  exibirMensagemSucesso(titulo: string, mensagem: string): Promise<SweetAlertResult> {
    return this.exibirMensagem({ titulo, mensagem, tipo: TipoAlertaEnum.Sucesso, textoBotaoOk: 'Ok' });
  }

  exibirMensagemErro(titulo: string, mensagem: string): Promise<SweetAlertResult> {
    return this.exibirMensagem({ titulo, mensagem, tipo: TipoAlertaEnum.Erro, textoBotaoOk: 'Ok' });
  }

  exibirMensagemAviso(titulo: string, mensagem: string): Promise<SweetAlertResult> {
    return this.exibirMensagem({ titulo, mensagem, tipo: TipoAlertaEnum.Aviso, textoBotaoOk: 'Ok' });
  }

  exibirMensagemInformativa(titulo: string, mensagem: string): Promise<SweetAlertResult> {
    return this.exibirMensagem({ titulo, mensagem, tipo: TipoAlertaEnum.Informacao, textoBotaoOk: 'Ok' });
  }

  exibirMensagemInterrogacaoSimNao(mensagem: string): Promise<SweetAlertResult> {
    return this.exibirMensagem({
      titulo: 'Atenção!',
      mensagem: mensagem,
      tipo: TipoAlertaEnum.Interrogacao,
      textoBotaoOk: 'Sim',
      textoBotaoCancelar: 'Não',
      exibirBotaoCancelar: true,
      estiloBotaoOk: 'btn btn-success'
    });
  }

  private assignDefined(target, ...sources) {
    for (const source of sources) {
      for (const key of Object.keys(source)) {
        const val = source[key];
        if (val !== undefined) {
          target[key] = val;
        }
      }
    }
    return target;
  }
}
