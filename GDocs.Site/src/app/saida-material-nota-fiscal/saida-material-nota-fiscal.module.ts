import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SaidaMaterialNotaFiscalRoutingModule } from './saida-material-nota-fiscal-routing.module';
import { ConsultaComponent } from './consulta/consulta.component';
import { SharedModule } from '../shared';
import { CadastroComponent } from './cadastro/cadastro.component';
import { TabelaItemMaterialComponent } from './tabela-item-material/tabela-item-material.component';
import { ModalAdicionarItemComponent } from './tabela-item-material/modal-adicionar-item/modal-adicionar-item.component';
import { ConsultaTabelaComponent } from './consulta-tabela/consulta-tabela.component';
import { UploadNotaFiscalComponent } from './upload-nota-fiscal/upload-nota-fiscal.component';
import { InformacaoSaidaMaterialComponent } from './upload-nota-fiscal/informacao-saida-material/informacao-saida-material.component';
import { PdfComponent } from './pdf/pdf.component';
import { SaidaMaterialNotaFiscalAcaoComponent } from './saida-material-nota-fiscal-acao/saida-material-nota-fiscal-acao.component';
import { HistoricoNotaFiscalComponent } from './historico-nota-fiscal/historico-nota-fiscal.component';
import { HistoricoNotaFiscalAnexoSaidaTabelaComponent } from './historico-nota-fiscal/historico-nota-fiscal-anexo-saida-tabela/historico-nota-fiscal-anexo-saida-tabela.component';

@NgModule({
  declarations: [
    ConsultaComponent,
    CadastroComponent,
    TabelaItemMaterialComponent,
    ModalAdicionarItemComponent,
    ConsultaTabelaComponent,
    UploadNotaFiscalComponent,
    InformacaoSaidaMaterialComponent,
    PdfComponent,
    SaidaMaterialNotaFiscalAcaoComponent,
    HistoricoNotaFiscalComponent,
    HistoricoNotaFiscalAnexoSaidaTabelaComponent],
  imports: [
    CommonModule,
    SharedModule,
    SaidaMaterialNotaFiscalRoutingModule
  ],
  exports:[
    TabelaItemMaterialComponent,
  ],
  entryComponents:[
    ModalAdicionarItemComponent,
    ConsultaTabelaComponent,
    InformacaoSaidaMaterialComponent,
    TabelaItemMaterialComponent,
    SaidaMaterialNotaFiscalAcaoComponent,
    PdfComponent,
    HistoricoNotaFiscalComponent,
    HistoricoNotaFiscalAnexoSaidaTabelaComponent
  ]
})
export class SaidaMaterialNotaFiscalModule { }
