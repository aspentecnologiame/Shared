import { AfterViewInit, Component, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from '../../core';
import { Base64Helper } from '../../helpers';
import { PdfPreviewForDialogModel } from '../models/pdf-preview-for-dialog.model';

@Component({
  selector: 'app-pdf-preview-for-dialog',
  templateUrl: './pdf-preview-for-dialog.component.html',
  styleUrls: ['./pdf-preview-for-dialog.component.scss']
})
export class PdfPreviewForDialogComponent extends ComponentFormBase implements AfterViewInit {

  @ViewChild('pdfViewer') pdfViewer;

  pdfSrc: any;
  tituloModal: string;

  constructor(
    public dialogRef: MatDialogRef<PdfPreviewForDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public pdfPreviewForDialogModel: PdfPreviewForDialogModel
  ) {
    super();
  }

  ngAfterViewInit(): void {
    this.tituloModal = this.obterTitulo();
    this.pdfViewer.pdfSrc = Base64Helper.criarArquivoPdf(this.pdfPreviewForDialogModel.arquivoBinario);
    this.pdfViewer.downloadFileName = this.obterDownloadFileName();
    this.pdfViewer.refresh();
  }

  obterTitulo(): string {
    if (this.pdfPreviewForDialogModel.tituloModal !== undefined && this.pdfPreviewForDialogModel.tituloModal !== null && this.pdfPreviewForDialogModel.tituloModal !== '') {
      return this.pdfPreviewForDialogModel.tituloModal;
    }

    return 'PDF - Detalhes do documento';
  }

  obterDownloadFileName(): string {
    if (this.pdfPreviewForDialogModel.nomeDoArquivoParaDownload !== undefined &&
      this.pdfPreviewForDialogModel.nomeDoArquivoParaDownload !== null &&
      this.pdfPreviewForDialogModel.nomeDoArquivoParaDownload !== '') {
      return this.pdfPreviewForDialogModel.nomeDoArquivoParaDownload;
    }

    return `Detalhes do documento`;
  }

  fecharModal() {
    this.dialogRef.close();
  }
}
