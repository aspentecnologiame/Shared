import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AutenticacaoService } from './../services/autenticacao.service';
import { environment } from '../../../environments/environment';
import { ExibicaoDeAlertaService } from './../../core/services/exibicao-de-alerta';

@Injectable({
  providedIn: 'root'
})
export class PermitionGuard implements CanActivate {

  constructor(
    private readonly router: Router,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService
  ) { }

  canActivate(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean {
    const permissao = next.routeConfig.data['permissao'];

    if (!this.autenticacaoService.statusUsuarioAutenticado || this.autenticacaoService.tokenEstaExpirado) {
      return false;
    }

    if (!this.autenticacaoService.validarPermissao(permissao)) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Acesso não permitido.').then(_ => {
        this.router.navigate([environment.principalUrl], { queryParams: { returnUrl: state.url } });
      });

      return false;
    }

    return true;
  }
}
