import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AutenticacaoService, MenuModel, UserDataModel } from './../../autenticacao';
import { NotificacaoService } from 'src/app/notificacao/notificacao.service';

@Component({
  selector: 'app-header-bar',
  templateUrl: './header-bar.component.html',
  styleUrls: ['./header-bar.component.scss']
})
export class HeaderBarComponent {

  usuario: UserDataModel;
  menu: Array<MenuModel> = [];
  existemRelatoriosNaoLidos: boolean = false;

  constructor(
    private readonly autenticacaoService: AutenticacaoService,
    private readonly router: Router,
    private readonly notificacaoService: NotificacaoService
  ) {
    this.usuario = autenticacaoService.obterUsuarioLogado() || new UserDataModel();
    this.initMenu();
    this.obterQuantidadeNaoLidasPorIdUsuario();
  }

  logout(): void {
    this.autenticacaoService.logout();
  }

  perfil() {
    this.router.navigate(['/usuario-profile/me/']);
  }

  obterQuantidadeNaoLidasPorIdUsuario(): void {
    this.notificacaoService.obterQuantidadeNaoLidasPorIdUsuario(this.usuario.id).subscribe((qtde) => {
      this.existemRelatoriosNaoLidos = (qtde > 0);
    });
  }

  notificacaoUsuario(): void {
    this.router.navigate(['/notificacao/']);
  }

  private initMenu(): void {
    const menuRoot = this.usuario.menu.filter(m => m.idPai === undefined);
    menuRoot.forEach(men => {
      men.filhos = this.usuario.menu.filter(m => m.idPai === men.id);

      if (men.filhos) {
        men.filhos.sort(this.compararOrder);
        men.filhos.reverse();
      }
    });

    menuRoot.sort(this.compararOrder);
    menuRoot.reverse();
    this.menu = menuRoot;
  }

  private compararOrder(a: MenuModel, b: MenuModel) {
    return a.ordem - b.ordem;
  }
}
