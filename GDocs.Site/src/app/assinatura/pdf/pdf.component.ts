import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Fi1548Service } from 'src/app/FI1548';
import { DocumentoModel, TipoPagamentoModel } from 'src/app/FI1548/shared';
import { MotivoCancelamentoModel } from 'src/app/saida-material-nota-fiscal/shared/models/motivo-cancelamento-status-pagamento-model';
import { ComponentFormBase } from '../../core';
import { Base64Helper, FormValidationHelper } from '../../helpers';
import { AssinaturaService } from '../assinatura.service';


@Component({
  selector: 'app-pdf',
  templateUrl: './pdf.component.html',
  styleUrls: ['./pdf.component.scss'],
})
export class PdfComponent extends ComponentFormBase implements OnInit {
  @ViewChild('pdfViewer2') pdfViewer;

  pdfSrc2: any;
  nomeDownload: string;
  page = 1;

  novoDocumento: DocumentoModel;
  formDadosDocumento: FormGroup;
  listaStatus: TipoPagamentoModel[] = [];
  formValidationHelper: FormValidationHelper;
  motivoCancelamento: MotivoCancelamentoModel[] =[];


  constructor(
    private readonly serviceFi1548: Fi1548Service,
    private readonly assinaturaService: AssinaturaService,
    public readonly dialogRef: MatDialogRef<PdfComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.formInit();
    this.obterMotivoCancelamentoFi1548();

  }

  private formInit(): void {
    this.carregarPdf();
  }

  private carregarPdf(): void {
    this.serviceFi1548.obterPdf(this.data.id).subscribe((pdfDoc) => {
      this.pdfViewer.pdfSrc = Base64Helper.criarArquivoPdf(pdfDoc.pdfDocumentBase64);
      this.pdfViewer.downloadFileName = 'FI1548_' + this.data.id;
      this.pdfViewer.refresh();
    });
  }


  private obterMotivoCancelamentoFi1548(): void {
    this.assinaturaService.obterMotivoCancelamento(this.data.id).subscribe((result) => {
      if(result != null ){
        this.motivoCancelamento = result;
      }
    });
  }




  fecharModal(): void {
    this.dialogRef.close(true);
  }

}
