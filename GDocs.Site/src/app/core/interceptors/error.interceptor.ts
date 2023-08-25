import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { catchError, retryWhen } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AlertaModel, ExibicaoDeAlertaService, TipoAlertaEnum } from '../services/exibicao-de-alerta';
import { genericRetryStrategy } from './../../helpers/rxjs/genericRetryStrategy';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private readonly router: Router,
    protected exibicaoDeAlertaService: ExibicaoDeAlertaService
  ) { }
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    return next.handle(request).pipe(
      retryWhen(genericRetryStrategy({ maxRetryAttempts: 2, excludedStatusCodes: [400, 401, 403, 404] })),
      catchError(err => {
        if (
          (request.url.toLowerCase().endsWith('/login') &&
            request.body.grantType &&
            request.body.grantType === 'refreshToken') ||
          err.status === 404
        ) {
          return throwError(err);
        }

        return this.handleError(err);
      })
    );
  }

  private handleError(httpErrorResponse: HttpErrorResponse) {
    const alert = new AlertaModel();
    alert.titulo = 'Erro';
    alert.mensagem = `Por favor, tente novamente.<br />
    Caso o problema persista entre em contato com o Suporte ICE: <a href="http://www.icecards.com.br" target="_blank">www.icecards.com.br</a>`;
    alert.tipo = TipoAlertaEnum.Erro;
    alert.textoBotaoOk = 'Ok';

    switch (httpErrorResponse.status) {
      case 400:
        if (httpErrorResponse.error.errors != undefined && httpErrorResponse.error.errors != null) {
          alert.titulo = 'Atenção!';
          alert.mensagem = httpErrorResponse.error.errors.map(function (item) {
            return item['message'];
          }).join('<br>');
          alert.tipo = TipoAlertaEnum.Aviso;
        } else {
          console.error(
            `Backend returned code ${httpErrorResponse.status}, ` +
            `body was: ${httpErrorResponse.error}`);
        }
        break;
      // 401 -> Não autorizado
      case 401:
        this.router.navigate([environment.loginUrl]);
        alert.titulo = 'Atenção!';
        alert.mensagem = 'A sua sessão expirou.';
        alert.tipo = TipoAlertaEnum.Aviso;
        break;

      // 403 -> Proibido
      case 403:
        this.router.navigate([environment.principalUrl]);
        alert.titulo = 'Atenção!';
        alert.mensagem = 'Acesso não permitido.';
        alert.tipo = TipoAlertaEnum.Aviso;
        break;

      default:
        break;
    }

    this.exibicaoDeAlertaService.exibirMensagem(alert);
    return throwError(alert);
  }
}
