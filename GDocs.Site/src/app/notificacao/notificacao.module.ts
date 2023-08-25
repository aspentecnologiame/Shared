import { NgModule } from '@angular/core';
import { SharedModule } from '../shared';
import { CommonModule } from '@angular/common';
import { NotificacaoRoutingModule } from './notificacao-routing.module';
import { NotificacaoUsuarioComponent } from './notificacao-usuario/notificacao-usuario.component';
import { PdfComponent } from './pdf/pdf.component';

@NgModule({
  declarations: [NotificacaoUsuarioComponent, PdfComponent],
  imports: [
    SharedModule,
    CommonModule,
    NotificacaoRoutingModule
  ],
  entryComponents:[
    PdfComponent
  ]
})
export class NotificacaoModule { }
