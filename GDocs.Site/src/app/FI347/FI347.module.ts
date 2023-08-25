
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from '../shared';
import { DetalheSaidaMaterialComponent } from '../shared/detalhe-saida-material/detalhe-saida-material.component';
import { AdicionarComponent } from './adicionar/adicionar.component';
import { ConsultaTabelaComponent } from './consulta-tabela/consulta-tabela.component';
import { ConsultaComponent } from './consulta/consulta.component';
import { Fi347RoutingModule } from './FI347-routing.module';
import { HistoricoMaterialComponent } from './historico-material/historico-material.component';
import { MaterialSaidaAcaoComponent } from './material-saida-acao/material-saida-acao.component';
import { PdfComponent } from './pdf/pdf.component';

@NgModule({
  declarations: [
    ConsultaComponent,
    AdicionarComponent,
    ConsultaTabelaComponent,
    MaterialSaidaAcaoComponent,
    PdfComponent,
    HistoricoMaterialComponent,
  ],
  imports: [
    CommonModule,
    Fi347RoutingModule,
    SharedModule,
  ],
  entryComponents:[
    MaterialSaidaAcaoComponent,
    HistoricoMaterialComponent,
    DetalheSaidaMaterialComponent,
    PdfComponent,
  ]
})
export class Fi347Module { }
