import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from '../../core';
import { Base64Helper, FormValidationHelper } from '../../helpers';
import { NotificacaoService } from '../notificacao.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-pdf',
  templateUrl: './pdf.component.html',
  styleUrls: ['./pdf.component.scss']
})
export class PdfComponent extends ComponentFormBase implements OnInit {

  @ViewChild('pdfView') pdfView;

  pdfSrc: any;
  nomeDownload: string;
  page = 1;

  formDadosDocumento: FormGroup;
  formValidationHelper: FormValidationHelper;

  constructor(
    public readonly dialogRef: MatDialogRef<PdfComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private readonly notificacaoService: NotificacaoService,
    private readonly router: Router,
  ) { 
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.visualizarPdf();
  }

  private visualizarPdf(): void {
    this.notificacaoService.obterNaoLidaPorIdNotificacao(this.data.idNotificacaoUsuario).subscribe((notificacaoRelatorio) => {
      this.pdfView.pdfSrc = Base64Helper.criarArquivoPdf(notificacaoRelatorio.rdlBytes);
      this.pdfView.downloadFileName = 'notificacao_usuario';
      this.pdfView.refresh();
    });
  }

  lido(): void {
    this.notificacaoService.atualizarPorIdNotificacaoUsuario(this.data.idNotificacaoUsuario).subscribe((resposta) => {
      this.dialogRef.close(true);
      this.router.navigate(['/notificacao/']);
    });
  }

  fechar(): void {
    this.dialogRef.close(false);
  }

}
