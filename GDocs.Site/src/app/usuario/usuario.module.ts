import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from '../shared';
import { UsuarioCadastroComponent } from './usuario-cadastro/usuario-cadastro.component';
import { UsuarioListaComponent } from './usuario-lista/usuario-lista.component';
import { UsuarioTabelaComponent } from './usuario-lista/usuario-tabela/usuario-tabela.component';
import { UsuarioProfileCriarAssinaturaComponent } from './usuario-profile/modal-criar-assinatura/usuario-profile-criar-assinatura.component';
import { UsuarioProfileComponent } from './usuario-profile/usuario-profile.component';
import { UsuarioRoutingModule } from './usuario-routing.module';


@NgModule({
  declarations: [
    UsuarioCadastroComponent,
    UsuarioListaComponent,
    UsuarioTabelaComponent,
    UsuarioProfileComponent
  ],
  imports: [
    CommonModule,
    UsuarioRoutingModule,
    SharedModule
  ],
  entryComponents: [UsuarioProfileCriarAssinaturaComponent]
})
export class UsuarioModule { }
