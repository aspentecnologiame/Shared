import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from '../shared';
import { UsuarioProfileCriarAssinaturaComponent } from './usuario-profile/modal-criar-assinatura';
import { UsuarioProfileUploadAssinaturaComponent } from './usuario-profile/modal-upload-assinatura';
import { UsuarioRoutingModule } from './usuario-routing.module';


@NgModule({
  declarations: [
    UsuarioProfileCriarAssinaturaComponent,
    UsuarioProfileUploadAssinaturaComponent
  ],
  imports: [
    CommonModule,
    UsuarioRoutingModule,
    SharedModule
  ],
  entryComponents: [
    UsuarioProfileCriarAssinaturaComponent,
    UsuarioProfileUploadAssinaturaComponent
  ]
})
export class UsuarioProfileModule { }
