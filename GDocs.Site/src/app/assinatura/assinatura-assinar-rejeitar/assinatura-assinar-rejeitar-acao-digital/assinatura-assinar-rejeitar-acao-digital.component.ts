import { Component,OnInit} from '@angular/core';
import { MatDialog, ThemePalette } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { AppConfig } from 'src/app/app.config';
import { AutenticacaoService } from 'src/app/autenticacao/services/autenticacao.service';
import { Base64Helper } from 'src/app/helpers';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { IFeatureCategorias } from 'src/app/shared/models/app-config-feature-categorias.model';
import { UploadResponseModel } from 'src/app/shared/models/upload-response.model';
import { AssinaturaService } from '../../assinatura.service';
import { AssinaturaPassoItemAssinarRejeitarModel } from '../../models';
import { AssinaturaAssinarRejeitarAcaoBaseComponent }
  from '../assinatura-assinar-rejeitar-acao-base/assinatura-assinar-rejeitar-acao-base.component';
import { UploadDocumentoAssinadoDigitalmenteModalComponent } from './upload-modal/upload-modal.component';
import { ExibicaoDeAlertaService, TipoAlertaEnum } from '../../../core';


@Component({
  selector: 'app-assinatura-assinar-rejeitar-acao-digital',
  templateUrl: './assinatura-assinar-rejeitar-acao-digital.component.html',
  styleUrls: ['./assinatura-assinar-rejeitar-acao-digital.component.scss']
})
export class AssinaturaAssinarRejeitarAcaoDigitalComponent extends AssinaturaAssinarRejeitarAcaoBaseComponent implements OnInit {
  uploadResponseModel: UploadResponseModel;
  botaoUploadHabiltado = false;
  FluxoPendenteOutroUsuario = false;
  checkFareiEnvioHabilitado = false;
  checkFareiEnvioValor = false;
  MostraFareiEnvio = false;
  color: ThemePalette = 'primary';
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

    this.configCategorias = AppConfig.settings.categorias;
    this.VerificaDocFoiAssinado();
    this.VerificarFareiEnvio();
    this.CategoriaEvent.subscribe((categoria) => {
      if (categoria != undefined)
        this.VerificarMostarFareiEnvio();
    });

     this.RejeitarAcao.subscribe(async acao  => {
      if (acao != undefined)
        await this.VerificaDocFoiAssinado();
    });

  }

  async downloadDocumentoUpadoNoFluxoAssinaturaParaAssinarDigitalmente() {
    await this.VerificaDocFoiAssinado();
    if (this.FluxoPendenteOutroUsuario === false)
      return

    this.GerarArquivoDownload(this.arquivoBinario);
    this.botaoUploadHabiltado = true;
    this.GravarCache();
  }



  uploadDocumentoAssinadoDigitalmente() {
    const dialogRef = this.dialog.open(UploadDocumentoAssinadoDigitalmenteModalComponent, {
      disableClose: true,
      height: '768',
      width: '800px'
    });

    dialogRef.afterClosed().subscribe((upload) => {
      if (upload !== null && upload !== undefined) {
        this.uploadResponseModel = upload;
        this.atualizarPdfViewerComDocumentoAssinadoDigitalmente(upload);
      }
    });
  }

  atualizarPdfViewerComDocumentoAssinadoDigitalmente(uploadResponseModel: UploadResponseModel) {
    this.pdfViewer.pdfSrc = Base64Helper.criarArquivoPdf(uploadResponseModel.arquivoBinario);
    this.pdfViewer.downloadFileName = uploadResponseModel.nomeSalvo;
    this.pdfViewer.refresh();
  }

  confirmarDocumentoAssinadoDigitalmente() {
    this.assinar(this.uploadResponseModel);
  }

  VerificarMostarFareiEnvio() {
    let categoria = this.configCategorias.find(w => w.codigo === this.categoria);

      if (categoria != undefined && categoria.flagFareiEnvio != undefined){
        if(categoria.flagFareiEnvio.habilitado){
          this.MostraFareiEnvio = true;
    }}
}





  FareiEnvioChange(event: any) {
    this.GravarFareiEnvio(this.processoAssinaturaDocumentoId, event.checked);
  }


  private AlertaDownloadAutomatico() {

    this.assinaturaService.BaixarDocumento(this.processoAssinaturaDocumentoId)
    .subscribe((result) => {
      if(result[0] != undefined){
        this.MensagemDownloadAutomatico(result[0].arquivoBinario)
      }
    })
  }


  private MensagemDownloadAutomatico(arquivo){
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: `O download do documento será feito agora. Você também receberá um e-mail com o mesmo documento em anexo`,
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    })
    .then(() =>
    {
      this.GerarArquivoDownload(arquivo)
    });

  }



  confirmarDocumentoAssinadoDigitalmenteComObservacao() {
    const dialogRef = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '280px',
      disableClose: true,
      data: {
        titulo: 'Observação'
      }
    });

    dialogRef.afterClosed().subscribe(retorno => {
      if (retorno === undefined || retorno.justificativa === null) {
        return;
      }

      this.assinar(this.uploadResponseModel, retorno.justificativa);
    });
  }

  assinar(uploadReponseModel, justificativa = '') {
    this.assinaturaService.assinar(new AssinaturaPassoItemAssinarRejeitarModel([this.processoAssinaturaDocumentoId], justificativa, uploadReponseModel))
      .subscribe(() => {
        this.assinaturaService.removerArquivoPendenteDeAssinaturaJaVisualizado(this.processoAssinaturaDocumentoId);
        this.LimparCacheDownload();
        this.AlertaDownloadAutomatico();
        this.navegarParaConsulta();
      },
        () => {
          this.assinaturaService.exbirMensagemNaoEPossivelAssinar();
        });
  }

  GravarCache() {
    this.assinaturaService.GravarCacheDownLoad(this.processoAssinaturaDocumentoId).subscribe();
  }

  async VerificaDocFoiAssinado() {
    await this.assinaturaService.ListarCacheDownload(this.processoAssinaturaDocumentoId)
      .toPromise()
      .then(result => {
        if (result.usuarioId !== this.autenticacaoService.obterUsuarioLogado().id) {
          this.bloquearRejeitar = true;
          this.FluxoPendenteOutroUsuario = false;
          this.assinaturaService.exbirMensagemNaoEhPosivelFazerDownload(result);
        }
        else
          this.FluxoPendenteOutroUsuario = true
      },
        () => {
          this.FluxoPendenteOutroUsuario = true
        });
  }

  DownloadDocumentoAutomatico()
  {
    }



  LimparCacheDownload() {
    this.assinaturaService.LimparCacheDownload(this.processoAssinaturaDocumentoId).subscribe()
  }


  GravarFareiEnvio(padid, fareienvio) {
    this.assinaturaService.GravarFareiEnvio(padid, fareienvio)
      .subscribe(() => { },
        () => {
          this.VerificarFareiEnvio();
        }
      );
  }

  async VerificarFareiEnvio() {
    await this.assinaturaService.listarPassoAssinanteRepresentanteProcesso(this.processoAssinaturaDocumentoId,true)
      .toPromise()
      .then(result => {
        if (result.find(x => x.fareiEnvio)) {
          if (result.find(x => x.fareiEnvio && (x.usuarioAdAssinanteGuid == this.autenticacaoService.obterUsuarioLogado().id || x.usuarioAdRepresentanteGuid == this.autenticacaoService.obterUsuarioLogado().id)))
            this.checkFareiEnvioValor = true;
          else {
            this.checkFareiEnvioHabilitado = true;
            this.checkFareiEnvioValor = false;
          }
        }
      });
  }

  private GerarArquivoDownload(binario) {
    let blob = Base64Helper.toBlob(binario, 'application/pdf');
    const fakelink = document.createElement("a");
    document.body.appendChild(fakelink);

    let url = window.URL.createObjectURL(blob);

    fakelink.href = url;
    fakelink.href = url;
    fakelink.download = String(this.arquivoDownloadFileName);
    fakelink.click();

    window.URL.revokeObjectURL(url);
    fakelink.remove();
  }

}
