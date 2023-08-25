import { UploadNotaFiscalComponent } from './upload-nota-fiscal/upload-nota-fiscal.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard, PermitionGuard } from '../autenticacao';
import { CadastroComponent } from './cadastro/cadastro.component';
import { ConsultaComponent } from './consulta/consulta.component';


const routes: Routes = [
  {
    path: '', component: ConsultaComponent,
    data: { permissao:'SaidaMaterialNF:consulta' ,breadcrumb: 'Consultar' }
  }
  ,{
    path: 'cadastro', component: CadastroComponent,
    data: { breadcrumb: 'Cadastrar documento', permissao: 'SaidaMaterialNF:acao:adiciona' }, canActivate: [AuthGuard, PermitionGuard]
  },
  {
    path: 'uploadNotaFiscal/:smnfId', component: UploadNotaFiscalComponent,
    data: { breadcrumb: 'Upload nota fiscal', permissao: 'SaidaMaterialNF:consulta' }, canActivate: [AuthGuard, PermitionGuard]
  }
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SaidaMaterialNotaFiscalRoutingModule { }

