import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard, PermitionGuard } from '../autenticacao';
import { AssinaturaAssinarRejeitarAcaoComponent } from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-acao/assinatura-assinar-rejeitar-acao.component';
import { AssinaturaCadastroComponent } from './assinatura-cadastro/assinatura-cadastro.component';
import { AssinaturaEdicaoUploadComponent } from './assinatura-edicao/assinatura-edicao-upload.component';
import { AssinaturaGerenciamentoComponent } from './assinatura-gerenciamento/assinatura-gerenciamento.component';
import { AssinaturaAssinarRejeitarListaComponent } from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-lista/assinatura-assinar-rejeitar-lista.component';
import { AssinaturaAssinarRejeitarAcaoDigitalComponent }
from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-acao-digital/assinatura-assinar-rejeitar-acao-digital.component';

const routes: Routes = [
  {
    path: '', component: AssinaturaAssinarRejeitarListaComponent
  },
  {
    path: 'adicionar', component: AssinaturaCadastroComponent,
    data: { breadcrumb: 'Criar fluxo', permissao: 'assinatura:gerenciardocumentos:adicionar' }, canActivate: [AuthGuard, PermitionGuard]
  },
  {
    path: 'gerenciamento', component: AssinaturaGerenciamentoComponent,
    data: { breadcrumb: 'Consultar', permissao: 'assinatura:gerenciardocumentos' }, canActivate: [AuthGuard, PermitionGuard]
  },
  { path: 'edicao/:padid/:docid', component: AssinaturaEdicaoUploadComponent, data: { breadcrumb: 'Editar arquivos - Upload' } },
  {
    path: 'assinar-rejeitar-acao/:padid', component: AssinaturaAssinarRejeitarAcaoComponent,
    data: { breadcrumb: 'Aprovar', permissao: 'assinatura:pendencias' }, canActivate: [AuthGuard, PermitionGuard]
  },
  {
    path: 'assinar-rejeitar-acao-digital/:padid', component: AssinaturaAssinarRejeitarAcaoDigitalComponent,
    data: { breadcrumb: 'Aprovar', permissao: 'assinatura:pendencias' }, canActivate: [AuthGuard, PermitionGuard]
  },
  {
    path: 'pendencias', component: AssinaturaAssinarRejeitarListaComponent,
    data: { breadcrumb: 'Aprovar', permissao: 'assinatura:pendencias' }, canActivate: [AuthGuard, PermitionGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UsuarioRoutingModule { }
