import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UsuarioListaComponent } from './usuario-lista/usuario-lista.component';
import { UsuarioCadastroComponent } from './usuario-cadastro/usuario-cadastro.component';
import { UsuarioProfileComponent } from './usuario-profile/usuario-profile.component';

const routes: Routes = [
  { path: '', component: UsuarioListaComponent },
  { path: 'inserir', component: UsuarioCadastroComponent, data: { breadcrumb: 'Adicionar usuário' } },
  { path: 'alterar/:id', component: UsuarioCadastroComponent, data: { breadcrumb: 'Editar usuário' } },
  { path: 'me', component: UsuarioProfileComponent, data: { breadcrumb: 'Meus dados' } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsuarioRoutingModule { }
