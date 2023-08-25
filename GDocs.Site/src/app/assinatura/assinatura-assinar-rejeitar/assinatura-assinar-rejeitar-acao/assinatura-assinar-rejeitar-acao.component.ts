import { Component, OnInit } from '@angular/core';
import { MatDialog, ThemePalette } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { AppConfig } from 'src/app/app.config';
import { AutenticacaoService } from 'src/app/autenticacao/services/autenticacao.service';
import { Base64Helper } from 'src/app/helpers';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { IFeatureCategorias } from 'src/app/shared/models/app-config-feature-categorias.model';
import { ExibicaoDeAlertaService, TipoAlertaEnum } from '../../../core';
import { AssinaturaService } from '../../assinatura.service';
import { AssinaturaInformacoesModel, AssinaturaPassoItemAssinarRejeitarModel, AssinaturaUploadResponseModel } from '../../models';
import { AssinaturaAssinarRejeitarAcaoBaseComponent } from '../assinatura-assinar-rejeitar-acao-base/assinatura-assinar-rejeitar-acao-base.component';
import { AssinaturaAssinanteRepresentanteModel } from './../../models/assinatura-assinante-representante.model';
import { PdfPreviewForDialogComponent } from 'src/app/shared/pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { PdfPreviewForDialogModel } from 'src/app/shared/models/pdf-preview-for-dialog.model';
import { Fi1548Service } from 'src/app/FI1548/FI1548.service';
import { PdfComponent } from '../../pdf/pdf.component';
import { DocumentoFI1548CienciaModel } from '../../models/documento-fi1548-ciencia-model';



@Component({
  selector: 'app-assinatura-assinar-rejeitar-acao',
  templateUrl: './assinatura-assinar-rejeitar-acao.component.html',
  styleUrls: ['./assinatura-assinar-rejeitar-acao.component.scss']
})
export class AssinaturaAssinarRejeitarAcaoComponent extends AssinaturaAssinarRejeitarAcaoBaseComponent implements OnInit {
  MostraFareiEnvioSemCertificado = false;
  checkFareiEnvioValorSemCertificado = false;
  checkFareiEnvioHabilitadoSemCertificado = false;
  possuiSubstitutoCancelado = false;
  possuiSubstitutoRejeitado = false;
  statusCancelado = 'Cancelado';
  colorSemCertificado: ThemePalette = 'primary';
  assinaturaInformacoesModel: AssinaturaInformacoesModel = new AssinaturaInformacoesModel();
  assinaturaInformacoesRetorno: AssinaturaInformacoesModel = new AssinaturaInformacoesModel();
  documentoRetorno: any;

  configCategorias: IFeatureCategorias[];

  constructor(
    public readonly activedRoute: ActivatedRoute,
    public readonly router: Router,
    public dialog: MatDialog,
    public readonly assinaturaService: AssinaturaService,
    public readonly autenticacaoService: AutenticacaoService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
  ) {
    super(activedRoute, router, dialog, assinaturaService,autenticacaoService);
  }

  ngOnInit() {
    super.ngOnInit();
    this.ConfiguracaoInicial()
  }

   VerificarMostarFareiEnvioSemCertificado() {
     let docCategoria = this.configCategorias.find(w => w.codigo === this.categoria);

    if (docCategoria != undefined && docCategoria.flagFareiEnvio != undefined )
        if(docCategoria.flagFareiEnvio.habilitado){
          this.MostraFareiEnvioSemCertificado = true;
    }
  }
  
  visualizarDetalhesDoProcesso() {
    this.assinaturaInformacoesModel.numeroDocumento = this.processoAssinaturaDocumentoId;
    this.assinaturaService.obterReferenciaSubstitutoDocumento(this.assinaturaInformacoesModel)
      .subscribe(
        response => {
          if (response.length >0) {
            this.assinaturaInformacoesRetorno = response[0];
            if(this.assinaturaInformacoesRetorno.status === this.statusCancelado){
              this.possuiSubstitutoCancelado = true
            }else{
              this.possuiSubstitutoRejeitado = true
            }
            this.obterDocumentoSubstituído()
            
          }
          
        }
      );

  }

  obterDocumentoSubstituído(){

    this.assinaturaService.obterDocumentoFI1548(this.assinaturaInformacoesRetorno.numeroDocumento)
    .subscribe(
      response => {

        this.documentoRetorno = response;
      })
    
  }

  exibirDetalhesDocumento(idDocumento: any): void {
    this.dialog.open(PdfComponent, {
      width: '920px',
      data: idDocumento,
      disableClose: true,
    });
  }

  
  FareiEnvioChangeSemCertificado(event: any) {
    this.assinaturaService.GravarFareiEnvio(this.processoAssinaturaDocumentoId, event.checked)
      .subscribe(() => { },
        () => {
          this.VerificarEuFareiEnvio();
        }
      );
  }
  

  async VerificarEuFareiEnvio() {
    await this.assinaturaService.listarPassoAssinanteRepresentanteProcesso(this.processoAssinaturaDocumentoId,true)
      .toPromise()
      .then(result => {
       this.AlteraEstadoEuFareiEnvio(result);
      });
  }

   assinar(justificativa = '') {
    this.assinaturaService.assinar(new AssinaturaPassoItemAssinarRejeitarModel([this.processoAssinaturaDocumentoId], justificativa, null)).subscribe(() => {
      this.assinaturaService.removerArquivoPendenteDeAssinaturaJaVisualizado(this.processoAssinaturaDocumentoId);
      this.DownloadAutomatico();
      this.navegarParaConsulta();
    },
      () => {
        this.assinaturaService.exbirMensagemNaoEPossivelAssinar();
      });
  }

  assinarComObservacao() {
    const dialogRef = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '285px',
      disableClose: true,
      data: {
        titulo: 'Observação da aprovação',
        confirmar: 'Aprovar com observação'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === undefined || result.justificativa === null) {
        return;
      }

      this.assinar(result.justificativa);
    });
  }

  private DownloadAutomatico() {
    this.assinaturaService.BaixarDocumento(this.processoAssinaturaDocumentoId)
    .subscribe((result) => {
      if(result[0] != undefined){
        this.AlertaDownloadAutomatico(result[0].arquivoBinario)
      }
    })
  }
  MostraAprovadoresPrevio():boolean{
    return this.aprovadoresPrevios != null
  }
  private AlertaDownloadAutomatico(arquivo){
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: `O download do documento será feito agora. Você também receberá um e-mail com o mesmo documento em anexo`,
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    })
    .then(() =>
    {
      this.GerarArquivo(arquivo)
    });

  }

  private GerarArquivo(binario) {
    let arquivo = Base64Helper.toBlob(binario, 'application/pdf');
    const linkfalso = document.createElement("a");
    document.body.appendChild(linkfalso);
    let url = window.URL.createObjectURL(arquivo);
    linkfalso.href = url;
    linkfalso.href = url;
    linkfalso.download = String(this.arquivoDownloadFileName);
    linkfalso.click();
    window.URL.revokeObjectURL(url);
    linkfalso.remove();
  }





  AlteraEstadoEuFareiEnvio(modelAssinante:AssinaturaAssinanteRepresentanteModel[])
  {
    if (modelAssinante.find(x => x.fareiEnvio)) {
      if (modelAssinante.find(x => x.fareiEnvio && (x.usuarioAdAssinanteGuid == this.autenticacaoService.obterUsuarioLogado().id || x.usuarioAdRepresentanteGuid == this.autenticacaoService.obterUsuarioLogado().id)))
        this.checkFareiEnvioValorSemCertificado = true;
      else {
        this.checkFareiEnvioHabilitadoSemCertificado = true;
        this.checkFareiEnvioValorSemCertificado = false;
      }
    }
  }
  VerificarDocumentoSubstituto()
  {
    this.assinaturaService

  }

  ConfiguracaoInicial()
  {
    this.configCategorias = AppConfig.settings.categorias;
    this.VerificarEuFareiEnvio();
    this.CategoriaEvent.subscribe((docCategoria) => {
      if (docCategoria != undefined)
        this.VerificarMostarFareiEnvioSemCertificado();
    });

    this.visualizarDetalhesDoProcesso()

   
   
  

   
  }

}
