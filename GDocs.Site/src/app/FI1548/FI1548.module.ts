import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AssinaturaModule } from '../assinatura/assinatura.module';
import { SharedModule } from '../shared';
import { CadastroFormularioComponent } from './cadastro/cadastro-formulario/cadastro-formulario.component';
import { CadastroUploadComponent } from './cadastro/cadastro-upload/cadastro-upload.component';
import { CadastroComponent } from './cadastro/cadastro.component';
import { ConsultaTabelaComponent } from './consulta-tabela/consulta-tabela.component';
import { ConsultaComponent } from './consulta/consulta.component';
import { Fi1548RoutingModule } from './FI1548-routing.module';
import { LiquidarComponent } from './liquidar/liquidar.component';
import { PdfComponent } from './pdf/pdf.component';

@NgModule({
  declarations: [
    CadastroComponent,
    ConsultaComponent,
    PdfComponent,
    ConsultaComponent,
    ConsultaTabelaComponent,
    LiquidarComponent,
    CadastroFormularioComponent,
    CadastroUploadComponent,
  ],
  imports: [
    CommonModule,
    Fi1548RoutingModule,
    SharedModule,
    AssinaturaModule,
  ],
  entryComponents: [
    PdfComponent
  ]
})
export class Fi1548Module { }
