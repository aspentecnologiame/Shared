import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CadastroComponent } from './cadastro/cadastro.component';
import { ConsultaComponent } from './consulta/consulta.component';
import { AuthGuard, PermitionGuard } from '../autenticacao';
import { LiquidarComponent } from './liquidar/liquidar.component';

const routes: Routes = [
  {
    path: '', component: ConsultaComponent,
    data: { breadcrumb: 'Consultar'}
  },
  {
    path: 'adicionar/:dfi_idt', component: CadastroComponent,
    data: { breadcrumb: 'Adicionar documento', permissao: 'fi1548:adicionar' }, canActivate: [AuthGuard, PermitionGuard]
  },
  {
    path: 'liquidar/:numero', component: LiquidarComponent,
    data: { breadcrumb: 'Liquidar documento', permissao: 'fi1548:consultar:liquidar' }, canActivate: [AuthGuard, PermitionGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class Fi1548RoutingModule { }
