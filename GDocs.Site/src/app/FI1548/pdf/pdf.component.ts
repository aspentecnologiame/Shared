import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { CienciaUsuarioAprovacaoModel } from 'src/app/saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model';
import { AutenticacaoService } from '../../autenticacao/services/autenticacao.service';
import { ComponentFormBase } from '../../core';
import { Base64Helper, FormValidationHelper } from '../../helpers';
import { Fi1548Service } from '../FI1548.service';
import { DocumentoModel, TipoPagamentoModel } from '../shared';
import { StatusDocumento } from '../shared/enums/status-documento';

@Component({
  selector: 'app-pdf',
  templateUrl: './pdf.component.html',
  styleUrls: ['./pdf.component.scss'],
})
export class PdfComponent extends ComponentFormBase implements OnInit,AfterViewInit {
  @ViewChild('pdfViewer') pdfViewer;

  pdfSrc: any;
  nomeDownload: string;
  page = 1;

  novoDocumento: DocumentoModel;
  formDadosDocumento: FormGroup;
  listaStatus: TipoPagamentoModel[] = [];
  formValidationHelper: FormValidationHelper;
  cienciaRejeitada:CienciaUsuarioAprovacaoModel[] =[]

  constructor(
    private readonly service: Fi1548Service,
    private readonly autenticacaoService: AutenticacaoService,
    public readonly dialogRef: MatDialogRef<PdfComponent>,
    private readonly router: Router,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }
  ngAfterViewInit(): void {
    this.carregarPdf();
  }

  ngOnInit() {
    super.ngOnInit();
    this.obterCienciaRejeitadosFi1548();
  }


  private carregarPdf(): void {
      this.pdfViewer.pdfSrc = Base64Helper.criarArquivoPdf(this.data.binario);
      this.pdfViewer.downloadFileName = 'FI1548_' + this.data.id;
      this.pdfViewer.refresh();
  }

  private obterCienciaRejeitadosFi1548(): void {
    this.service.obterCienciaCancelada(this.data.id).subscribe((result) => {
      if(result != null ){
        this.cienciaRejeitada = result.cienciaUsuariosAprovacao;
      }
    });
  }

mostraEditarStatusPagamento():boolean{
  const statusPag: string = this.data.status;
  return  StatusDocumento[statusPag] === StatusDocumento.EmConstrucao;
}

  mostrarBotaoEnviarParaAssinatura() {
    const status: string = this.data.status;
    return StatusDocumento[status] === StatusDocumento.EmConstrucao &&
      (
        this.autenticacaoService.validarPermissao('fi1548:consultar:todosusuarios:enviarparaassinatura') ||
        this.autenticacaoService.obterUsuarioLogado().id === this.data.autorId
      )
  }

  fecharModal(): void {
    this.dialogRef.close(true);
  }

  enviarParaAssinatura(): void {
      this.service.obterOrigemPorId(this.data.id).subscribe((padId) => {
      this.router.navigate(['assinatura/edicao/',padId.processoAssinaturaDocumentoId,this.data.id]);
      this.dialogRef.close(false);
    });
  }


  enviarParaEdicaoStatusPagamento(): void {
    this.router.navigate(['fi1548/adicionar/',this.data.id]);
    this.dialogRef.close(false);
  }
}
