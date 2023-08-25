import { HttpEventType } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, last } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { GDocsApiEndpointService } from "../core/services/gdocs-api/gdocs-api-endpoint.service";
import { NotificacaoRelatorioModel } from './models/notificacao-relatorio.model';

@Injectable({
  providedIn: 'root'
})
export class NotificacaoService {

  private readonly URLS = {
    qtdeNotificacoesNaoLidasPorIdUsuario: "/v1/notificacaousuario/obterquantidadenaolidasporidusuario",
    notificacoesNaoLidasPorIdUsuario: "/v1/notificacaousuario/obternaolidasporidusuario",
    notificacoesNaoLidaPorIdNotificacao: "/v1/notificacaousuario/obternaolidaporidnotificacao",
    atualizarPorIdNotificacaoUsuario: "/v1/notificacaousuario/atualizarporidnotificacaousuario"
  }
  constructor(
    private readonly gDocsApiEndpointService: GDocsApiEndpointService
  ) { }

  private mapProcessList<T>(event: any): Array<T> {
    if (event.type === HttpEventType.Response) {
      return Object.assign([], event.body);
    }

    return undefined;
  }

  private mapProcess<T>(event: any): T {
    if (event.type === HttpEventType.Response) {
      return event.body as T;
    }
    return undefined;
  }

  obterQuantidadeNaoLidasPorIdUsuario(idUsuario: string): Observable<number> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.qtdeNotificacoesNaoLidasPorIdUsuario + `?idUsuario=${idUsuario}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterNaoLidasPorIdUsuario(idUsuario: string): Observable<NotificacaoRelatorioModel[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.notificacoesNaoLidasPorIdUsuario + `?idUsuario=${idUsuario}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterNaoLidaPorIdNotificacao(idNotificacaoUsuario: number): Observable<NotificacaoRelatorioModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.notificacoesNaoLidaPorIdNotificacao + `?idNotificacaoUsuario=${idNotificacaoUsuario}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  atualizarPorIdNotificacaoUsuario(idNotificacaoUsuario: number): Observable<number> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.atualizarPorIdNotificacaoUsuario, idNotificacaoUsuario)
      .pipe(
        last(),
        map((event) => this.mapProcess<number>(event))
      );
  }
}
