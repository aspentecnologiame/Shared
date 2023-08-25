import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
import { Subject } from 'rxjs';
import { ExibicaoDeAlertaService } from '../../core/services/exibicao-de-alerta/exibicao-de-alerta.service';
import { Base64Helper } from '../../helpers';
import { UploadResponseModel } from '../models/upload-response.model';

@Component({
  selector: 'app-pdf-preview',
  templateUrl: './pdf-preview.component.html',
  styleUrls: ['./pdf-preview.component.scss']
})
export class PdfPreviewComponent implements OnInit {

  @ViewChild('pdfViewer') pdfViewer;
  @Input() listarDocumentosChanging: Subject<UploadResponseModel[]>
  @Input() showThumbnail;
  @Input() bloquearRemocaoPrimeiroElemento: false;
  @Input() mostraTrash: boolean = true;
  @Input() detalheDocumento: boolean = true;
  @Output() docSelecionadoEvent = new EventEmitter<UploadResponseModel>();

  listaUploadDocumentos: UploadResponseModel[] = [];
  pdfSrc: any;
  page = 1;
  nomeDownload: string;

  public config: PerfectScrollbarConfigInterface = {};

  constructor(
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService
  ) { }

  ngOnInit() {
    this.listarDocumentosChanging.subscribe(documento => {
      this.listarDocumentos(documento);
    });
  }

  private listarDocumentos(listaUploadDocumentos: UploadResponseModel[]): void {
    this.listaUploadDocumentos = listaUploadDocumentos;
    this.listaUploadDocumentos.forEach(documento => {
      documento.pdfCaminhoArquivo = Base64Helper.criarArquivoPdf(documento.arquivoBinario);
      documento.selecionado = false;
    });

    this.visualizarImagem(this.listaUploadDocumentos[0]);
  }

  private desmarcarVisualizacao() {
    this.listaUploadDocumentos.forEach(documento => {
      documento.selecionado = false;
    });
  }

  visualizarImagem(documento: UploadResponseModel) {
    this.docSelecionadoEvent.emit(documento);
    this.desmarcarVisualizacao();
    documento.selecionado = true;
    this.pdfViewer.pdfSrc = documento.pdfCaminhoArquivo;
    this.pdfViewer.downloadFileName = documento.nomeOriginal;
    this.pdfViewer.refresh();
  }

  removerDocumento(index: number) {
    if (this.bloquearRemocaoPrimeiroElemento && index === 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Não é permitido remover esse arquivo');
      return;
    }

    this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja remover o arquivo ${this.listaUploadDocumentos[index].nomeOriginal}?`)
      .then(resposta => {
        if (resposta.value) {
          this.listaUploadDocumentos.splice(index, 1);
          if (this.listaUploadDocumentos.length > 0) {
            this.visualizarImagem(this.listaUploadDocumentos[0]);
          }
        }
      });
  }

  removerTodosDocumentos() {
    this.listaUploadDocumentos = [];
  }
}
