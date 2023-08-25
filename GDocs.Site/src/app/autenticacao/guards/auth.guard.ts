import { environment } from './../../../environments/environment';
import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AutenticacaoService } from '../services/autenticacao.service';
import { ExibicaoDeAlertaService } from './../../core/services/exibicao-de-alerta';
import { split } from 'lodash';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(
    private readonly router: Router,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService
  ) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {

    const keyNameGuidSignature = 'chave';
    if (!this.autenticacaoService.statusUsuarioAutenticado && next.queryParams[keyNameGuidSignature]) {
      return new Promise<boolean>((resolve) => {
        const url = split(state.url, '?')[0];
        this.autenticacaoService.loginGuidSignature(next.queryParams[keyNameGuidSignature]).subscribe(
          _ => {
            const objectQueryParams = this.getObjectQueryParamsWithoutGuidSignature(next, keyNameGuidSignature);

            this.router.navigate([url], { queryParams: objectQueryParams });
            resolve(true);
          },
          _ => {
            const stringQueryParams = this.getStringQueryParamsWithoutGuidSignature(next, keyNameGuidSignature);

            this.router.navigate([environment.loginUrl], { queryParams: { returnUrl: `${url}${stringQueryParams}` } });
            resolve(false);
          }
        );
      });
    }

    if (!this.autenticacaoService.statusUsuarioAutenticado) {
      this.router.navigate([environment.loginUrl], { queryParams: { returnUrl: state.url } });
      return false;
    }

    if (this.autenticacaoService.tokenEstaExpirado) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'A sua sessão expirou.').then(_ => {
        this.router.navigate([environment.loginUrl], { queryParams: { returnUrl: state.url } });
      });
      return false;
    }

    return true;
  }

  getObjectQueryParamsWithoutGuidSignature(next: ActivatedRouteSnapshot, key: string) {
    const queryParams = new Object();
    for (const param in next.queryParams) {
      if (param !== key) {
        queryParams[param] = next.queryParams[param];
      }
    }

    return queryParams;
  }

  getStringQueryParamsWithoutGuidSignature(next: ActivatedRouteSnapshot, key: string) {
    const listQueryParams = [];
    for (const param in next.queryParams) {
      if (param !== key) {
        listQueryParams.push(`${param}=${next.queryParams[param]}`);
      }
    }

    let stringQueryParams = '';
    if (listQueryParams.length > 0) {
      stringQueryParams = `?${listQueryParams.join('&')}`;
    }

    return stringQueryParams;
  }
}
