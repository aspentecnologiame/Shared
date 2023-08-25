import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Router } from '@angular/router';
import { AutenticacaoService } from 'src/app/autenticacao';
import { CienciaUsuarioAprovacaoModel } from 'src/app/saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model';
import { ComponentFormBase } from '../../core';
import { Base64Helper, FormValidationHelper } from '../../helpers';
import { Fi1347Service } from '../FI347.service';
import { StatusSaidaMaterial } from '../shared/enums/status-saida-material';

@Component({
  selector: 'app-pdf',
  templateUrl: './pdf.component.html',
  styleUrls: ['./pdf.component.scss'],
})
export class PdfComponent extends ComponentFormBase implements OnInit,AfterViewInit {
  @ViewChild('pdfView') pdfView;

  pdfSrc: any;
  nomeDownload: string;
  page = 1;

  cienciaUsuarioRejeitado:CienciaUsuarioAprovacaoModel[] =[]

  formDadosDocumento: FormGroup;
  formValidationHelper: FormValidationHelper;

  constructor(
    private readonly service: Fi1347Service,
    public readonly dialogRef: MatDialogRef<PdfComponent>,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly router: Router,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }
  ngAfterViewInit(): void {
  this.visualizarPdf()
  }

  ngOnInit() {
    super.ngOnInit();
    this.obterCienciaRejeitados();
 }

  private visualizarPdf(): void {
    this.pdfView.pdfSrc = Base64Helper.criarArquivoPdf(this.data.binario);
    this.pdfView.downloadFileName = 'FI347_';
    this.pdfView.refresh();
  }

  private obterCienciaRejeitados(): void {
    this.service.obterAprovadoresCienciaCancelada(this.data.id).subscribe((result) => {
      if(result != null ){
        this.cienciaUsuarioRejeitado = result.cienciaUsuariosAprovacao;
      }
    });
  }




  fechar(): void {
    this.dialogRef.close(true);
  }

  mostrarBotaoEnviarMaterialParaAssinatura() {
    const status: number = this.data.statusId;
    return status === StatusSaidaMaterial.EmConstrucao &&
      (
        this.autenticacaoService.validarPermissao('fi347:enviar:assinatura') ||
        this.autenticacaoService.obterUsuarioLogado().id === this.data.autor
      )
  }

  enviarMaterialParaAssinatura(): void {
    this.service.enviarMaterialAssinatura(this.data).subscribe((padId) => {
      this.router.navigate(['assinatura/edicao/',padId,this.data.id]);
      this.dialogRef.close(false);
    });
  }


  enviarParaEdicao(): void {
    this.router.navigate(['fi347/adicionar/',this.data.id]);
    this.dialogRef.close(false);
  }
}
