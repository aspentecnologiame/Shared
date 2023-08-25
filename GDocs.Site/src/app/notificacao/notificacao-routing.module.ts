import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotificacaoUsuarioComponent } from './notificacao-usuario/notificacao-usuario.component';

const routes: Routes = [
  { path: '', component: NotificacaoUsuarioComponent }
];

@NgModule({
  declarations: [],
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NotificacaoRoutingModule { }
