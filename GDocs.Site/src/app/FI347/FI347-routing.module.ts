import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard, PermitionGuard } from '../autenticacao';
import { AdicionarComponent } from './adicionar/adicionar.component';
import { ConsultaComponent } from './consulta/consulta.component';

const routes: Routes = [
  {
    path: '', component: ConsultaComponent,
    data: { breadcrumb: 'Consultar' }
  },
  {
    path: 'adicionar/:ssmid', component: AdicionarComponent,
    data: { breadcrumb: 'Adicionar documento', permissao: 'fi347:adicionar' }, canActivate: [AuthGuard, PermitionGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class Fi347RoutingModule { }
