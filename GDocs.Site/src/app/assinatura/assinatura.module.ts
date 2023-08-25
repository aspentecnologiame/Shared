import { CommonModule } from '@angular/common';
import {  NgModule } from '@angular/core';
import { SharedModule } from '../shared';
import { AssinaturaAssinarRejeitarAcaoComponent } from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-acao/assinatura-assinar-rejeitar-acao.component';
import {
  AssinaturaAssinarRejeitarListaComponent
} from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-lista/assinatura-assinar-rejeitar-lista.component';
import {
  AssinaturaAssinarRejeitarTabelaComponent
} from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-lista/assinatura-assinar-rejeitar-tabela/assinatura-assinar-rejeitar-tabela.component';
import { AssinaturaAssinadoresCardComponent } from './assinatura-cadastro/assinatura-assinadores-card/assinatura-assinadores-card.component';
import { AssinaturaAssinadoresComponent } from './assinatura-cadastro/assinatura-assinadores/assinatura-assinadores.component';
import { AssinaturaCadastroComponent } from './assinatura-cadastro/assinatura-cadastro.component';
import { AssinaturaInformacoesComponent } from './assinatura-cadastro/assinatura-informacoes/assinatura-informacoes.component';
import { AssinaturaEdicaoUploadComponent } from './assinatura-edicao/assinatura-edicao-upload.component';
import { AssinaturaGerenciamentoComponent } from './assinatura-gerenciamento/assinatura-gerenciamento.component';

import {
  AssinaturaGerenciamentoPendenciaAssinaturaComponent,
  PassoComponent,
  PendenciaCardComponent,
  RepresentanteAutoCompleteComponent,
  AssinaturaGerenciamentoRepresentanteTabelaComponent,
  AssinaturaGerenciamentoRepresentanteComponent,
  AssinaturaGerenciamentoTabelaComponent
} from './assinatura-gerenciamento';

import { UsuarioRoutingModule } from './assinatura-routing.module';
import { AssinaturaAssinarRejeitarAcaoDigitalComponent }
from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-acao-digital/assinatura-assinar-rejeitar-acao-digital.component';
import { UploadDocumentoAssinadoDigitalmenteModalComponent }
from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-acao-digital/upload-modal/upload-modal.component';
import { AssinaturaCienciaAcaoComponent } from './assinatura-assinar-rejeitar/assinatura-ciencia-acao/assinatura-ciencia-acao.component';
import { AssinaturaAssinarRejeitarFooterComponent } from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-footer/assinatura-assinar-rejeitar-footer.component';
import { AssinaturaAssinarRejeitarCardJustificativaComponent } from './assinatura-assinar-rejeitar/assinatura-assinar-rejeitar-card-justificativa/assinatura-assinar-rejeitar-card-justificativa.component';
import { AssinaturaCienciaMaterialNotaFiscalAcaoComponent } from './assinatura-assinar-rejeitar/assinatura-ciencia-acao/assinatura-ciencia-material-nota-fiscal-acao/assinatura-ciencia-material-nota-fiscal-acao.component';
import { SaidaMaterialNotaFiscalModule } from '../saida-material-nota-fiscal';
import { AssinaturaCienciaAcaoCardJustificativaComponent } from './assinatura-assinar-rejeitar/assinatura-ciencia-acao/assinatura-ciencia-acao-card-justificativa/assinatura-ciencia-acao-card-justificativa.component';
import { AssinaturaCienciaStatusPagamentoAcaoComponent } from './assinatura-assinar-rejeitar/assinatura-ciencia-acao/assinatura-ciencia-status-pagamento-acao/assinatura-ciencia-status-pagamento-acao.component';
import { PdfComponent } from './pdf/pdf.component';

@NgModule({
  declarations: [
    AssinaturaCadastroComponent,
    AssinaturaInformacoesComponent,
    AssinaturaAssinadoresComponent,
    AssinaturaAssinadoresCardComponent,
    AssinaturaGerenciamentoComponent,
    AssinaturaGerenciamentoTabelaComponent,
    AssinaturaGerenciamentoRepresentanteComponent,
    AssinaturaGerenciamentoRepresentanteTabelaComponent,
    RepresentanteAutoCompleteComponent,
    AssinaturaGerenciamentoPendenciaAssinaturaComponent,
    PendenciaCardComponent,
    PassoComponent,
    AssinaturaEdicaoUploadComponent,
    AssinaturaAssinarRejeitarAcaoComponent,
    AssinaturaAssinarRejeitarListaComponent,
    AssinaturaAssinarRejeitarTabelaComponent,
    AssinaturaAssinarRejeitarAcaoDigitalComponent,
    UploadDocumentoAssinadoDigitalmenteModalComponent,
    AssinaturaCienciaAcaoComponent,
    AssinaturaAssinarRejeitarFooterComponent,
    AssinaturaAssinarRejeitarCardJustificativaComponent,
    AssinaturaCienciaMaterialNotaFiscalAcaoComponent,
    AssinaturaCienciaAcaoCardJustificativaComponent,
    AssinaturaCienciaStatusPagamentoAcaoComponent,
    PdfComponent
  ],
  imports: [
    CommonModule,
    UsuarioRoutingModule,
    SharedModule,
    SaidaMaterialNotaFiscalModule,
  ],
  exports:[
    AssinaturaAssinadoresComponent,
    AssinaturaEdicaoUploadComponent,
  ],
  entryComponents: [
    AssinaturaAssinadoresCardComponent,
    AssinaturaCienciaAcaoComponent,
    AssinaturaGerenciamentoRepresentanteComponent,
    AssinaturaGerenciamentoPendenciaAssinaturaComponent,
    UploadDocumentoAssinadoDigitalmenteModalComponent,
    AssinaturaCienciaMaterialNotaFiscalAcaoComponent,
    AssinaturaCienciaStatusPagamentoAcaoComponent,
    PdfComponent
  ]
})
export class AssinaturaModule { }
