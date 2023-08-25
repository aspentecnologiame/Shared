import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard, LoginComponent, PermitionGuard } from './autenticacao';
import { HomeLayoutComponent, LoginLayoutComponent } from './layouts';


const routes: Routes = [
  { path: '', redirectTo: '/assinatura', pathMatch: 'full' },
  {
    path: 'login',
    component: LoginLayoutComponent,
    children: [
      { path: '', component: LoginComponent },
    ]
  },
  {
    path: '',
    component: HomeLayoutComponent,
    children: [
      // Lazy Loading
      {
        path: 'usuario', loadChildren: './usuario/usuario.module#UsuarioModule',
        data: { permissao: 'usuario', breadcrumb: 'Usuários' }, canActivate: [AuthGuard, PermitionGuard]
      },
      {
        path: 'usuario-profile', loadChildren: './usuario/usuario-profile.module#UsuarioProfileModule',
        data: { breadcrumb: 'Usuários' }, canActivate: [AuthGuard]
      },
      {
        path: 'fi347', loadChildren: './FI347/FI347.module#Fi347Module',
        data: { permissao: 'fi347:consultar',  breadcrumb: 'Solicitações/Saída de material - sem NF' }, canActivate: [AuthGuard, PermitionGuard]
      },
      {
        path: 'SaidaMaterialNotaFiscal', loadChildren: './saida-material-nota-fiscal/saida-material-nota-fiscal.module#SaidaMaterialNotaFiscalModule',
        data: { permissao: 'SaidaMaterialNF:consulta',  breadcrumb: 'Solicitações/Saída de material - com NF' }, canActivate: [AuthGuard, PermitionGuard]
      },
      {
        path: 'notificacao', loadChildren: './notificacao/notificacao.module#NotificacaoModule',
        data: { permissao: 'Notificacoes:Relatorio',  breadcrumb: 'Notificações' }, canActivate: [AuthGuard, PermitionGuard]
      },
      {
        path: 'fi1548', loadChildren: './FI1548/FI1548.module#Fi1548Module',
        data: { permissao: 'fi1548:consultar', breadcrumb: 'Solicitações/Status - Pagamentos' }, canActivate: [AuthGuard, PermitionGuard]
      },
      {
        path: 'assinatura', loadChildren: './assinatura/assinatura.module#AssinaturaModule',
        data: { permissao: 'assinatura', breadcrumb: 'Aprovações' }, canActivate: [AuthGuard, PermitionGuard]
      },
    ]
  },
 { path: 'fi347', redirectTo: '/fi347', },
 { path: 'SaidaMaterialNotaFiscal', redirectTo: '/SaidaMaterialNotaFiscal', },
 { path: '**', redirectTo: '/fi1548', }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule { }
