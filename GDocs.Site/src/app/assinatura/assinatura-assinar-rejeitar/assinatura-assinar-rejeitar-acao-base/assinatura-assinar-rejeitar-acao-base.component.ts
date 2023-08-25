import { EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { CategoriaDocumento } from 'src/app/assinatura/enums/categoria-documento.enum';
import { AutenticacaoService } from 'src/app/autenticacao/services/autenticacao.service';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { ComponentFormBase } from '../../../core/component-form-base';
import { Base64Helper } from '../../../helpers';
import { AssinaturaService } from '../../assinatura.service';

import { AssinaturaStatusDocumento, AssinaturaStatusDocumentoLabel } from '../../enums/assinatura-status-documento-enum';
import { AssinaturaStatusPassoAtual, AssinaturaStatusPassoAtualLabel } from '../../enums/assinatura-status-passo-atual-enum';
import { StatusAssinaturaDocumentoPassoUsuario } from '../../enums/status-assinatura-documento-passo-usuario';
import { AssinaturaPassoItemAssinarRejeitarModel, AssinaturaUploadResponseModel, AssinaturaUsuarioAdModel } from '../../models';

export class AssinaturaAssinarRejeitarAcaoBaseComponent extends ComponentFormBase implements OnInit {
  @ViewChild('pdfViewer') pdfViewer;
  @Output() CategoriaEvent = new EventEmitter<number>();
  @Output() RejeitarAcao = new EventEmitter<boolean>();


  AssinaturaStatusDocumento: AssinaturaStatusDocumento;
  AssinaturaStatusPassoAtual: AssinaturaStatusPassoAtual;
  processoAssinaturaDocumentoId: number;
  categoria:number;
  listaDocumentosUpload: AssinaturaUploadResponseModel[] = [];
  listarDocumentosChangingValue: Subject<AssinaturaUploadResponseModel[]> = new Subject();
  assinaturaUsuarioAd: AssinaturaUsuarioAdModel[] = []
  aprovadoresPrevios: string;
  possuiAlgumaRejeicaoNoPasso = false;
  bloquearRejeitar = false;
  possui = false;
  textoAssinatura = "Aprovar";
  textoAssinaturaComObs = "Aprovado com observação";




  arquivoBinario: string;
  arquivoDownloadFileName: string;

  pdfSrc: any;
  nomeDownload: string;
  nomeAutor: string;

  constructor(
    public readonly activedRoute: ActivatedRoute,
    public readonly router: Router,
    public dialog: MatDialog,
    public readonly assinaturaService: AssinaturaService,
    public readonly autenticacaoService: AutenticacaoService
  ) {
    super();
   }


  ngOnInit() {
    const padid = this.activedRoute.snapshot.paramMap.get('padid');
    if (padid !== null && padid !== undefined && padid !== '' && !isNaN(Number(padid.toString()))) {
      this.processoAssinaturaDocumentoId = Number(padid);
      this.popularListAssinaturaDocumentosUpload();
    }

    super.ngOnInit();
  }
  popularListAssinaturaDocumentosUpload(): void {
    this.assinaturaService.assinarRejeitarValidarExisteAssinaturaDocJaAssinado([this.processoAssinaturaDocumentoId]).subscribe(
      () => {
        this.assinaturaService.listarArquivosUploadEPassoUsuario(this.processoAssinaturaDocumentoId).subscribe((arquivosUploadEPassoUsuario) => {
          if (arquivosUploadEPassoUsuario.assinaturaArquivos.length === 0 ||
            arquivosUploadEPassoUsuario.assinaturaArquivos.filter(arquivos => arquivos.processoAssinaturaDocumentoStatusId !== AssinaturaStatusDocumento.EmAndamento).length > 0) {
            this.assinaturaService.exbirMensagemNaoEPossivelAssinar();
          }

          this.arquivoBinario = arquivosUploadEPassoUsuario.assinaturaArquivos[0].arquivoBinario;
          this.arquivoDownloadFileName = 'documento_' + arquivosUploadEPassoUsuario.assinaturaArquivos[0].id;

          this.pdfViewer.pdfSrc = Base64Helper.criarArquivoPdf(arquivosUploadEPassoUsuario.assinaturaArquivos[0].arquivoBinario);
          this.pdfViewer.downloadFileName = this.arquivoDownloadFileName;
          this.pdfViewer.refresh();

          this.possuiAlgumaRejeicaoNoPasso = (arquivosUploadEPassoUsuario.assinaturaUsuarioAd.filter(assinaturasComJustificativas =>
            assinaturasComJustificativas.status === this.retornarDescricaoStatusAssinatura(AssinaturaStatusDocumento.ConcluidoComRejeicao)).length > 0);

          this.assinaturaUsuarioAd = arquivosUploadEPassoUsuario.assinaturaUsuarioAd.filter(assinaturasComJustificativas =>
            assinaturasComJustificativas.justificativa !== null &&
            assinaturasComJustificativas.justificativa !== '' &&
            assinaturasComJustificativas.justificativa !== undefined);

          if(arquivosUploadEPassoUsuario.assinaturaArquivos.every(x=> x.categoria == CategoriaDocumento.Statuspagamento)){
            
            this.formataAprovadoresPrevios(arquivosUploadEPassoUsuario.assinaturaUsuarioAd.filter(x => x.fluxoNotificacao != 'DIR-MESMO-PASSO' && StatusAssinaturaDocumentoPassoUsuario[x.status] == StatusAssinaturaDocumentoPassoUsuario.Assinado))
          }

          this.nomeAutor = arquivosUploadEPassoUsuario.assinaturaArquivos[0].autor;
          this.categoria = arquivosUploadEPassoUsuario.assinaturaArquivos[0].categoria;
          this.CategoriaEvent.emit(this.categoria);
          this.assinaturaService.marcarArquivoPendenteDeAssinaturaComoJaVisualizado(this.processoAssinaturaDocumentoId);
        });
      }
    );
  }


  formataAprovadoresPrevios(aprovadores :AssinaturaUsuarioAdModel[]){
    if(aprovadores.length > 0){
      let aprovadoresNome = aprovadores.map(x => x.nome);
      let resultado = '';
      if (aprovadoresNome.length === 1) {
        resultado = aprovadoresNome[0];
      } else if (aprovadoresNome.length === 2) {
        resultado = aprovadoresNome.join(' e ');
      } else if (aprovadoresNome.length > 2) {
        const ultimoItem = aprovadoresNome.pop();
        resultado = aprovadoresNome.join(', ') + ' e ' + ultimoItem;
      }
      this.aprovadoresPrevios = resultado ;
    }
  }
  retornarDescricaoStatusAssinatura(status: AssinaturaStatusDocumento) {
    return AssinaturaStatusDocumentoLabel.get(status);
  }

  retornarDescricaoStatusPassoAtual(status: AssinaturaStatusPassoAtual) {
    return AssinaturaStatusPassoAtualLabel.get(status);
  }

  async rejeitar() {

     await this.VerificaDocumentoAssinado()
     this.RejeitarAcao.emit(true);

     if(this.bloquearRejeitar)
      return;


    const dialogRef = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '285px',
      disableClose: true,
      data: {
        titulo: 'Observação para a rejeição',
        confirmar: 'Confirmar rejeição'
      }
    });

    dialogRef.afterClosed().subscribe(retorno => {
      if (retorno === undefined || retorno.justificativa === null) {
        return;
      }

      this.assinaturaService.rejeitar(new AssinaturaPassoItemAssinarRejeitarModel([this.processoAssinaturaDocumentoId], retorno.justificativa, null))
        .subscribe(() => {
          this.assinaturaService.removerArquivoPendenteDeAssinaturaJaVisualizado(this.processoAssinaturaDocumentoId);
          this.assinaturaService.LimparCacheDownload(this.processoAssinaturaDocumentoId).subscribe();
          this.navegarParaConsulta();
        }, () => {
          this.assinaturaService.exbirMensagemNaoEPossivelAssinar();
        });
    });
  }

  async VerificaDocumentoAssinado() {
    await this.assinaturaService.ListarCacheDownload(this.processoAssinaturaDocumentoId)
      .toPromise()
      .then(result => {
        if (result.usuarioId !== this.autenticacaoService.obterUsuarioLogado().id)
          this.bloquearRejeitar = true;
        else
          this.bloquearRejeitar = false
      }, () => {
          this.bloquearRejeitar = false
        });
  }

  navegarParaConsulta() {
    this.assinaturaService.LimparCacheDownload(this.processoAssinaturaDocumentoId).subscribe();
    this.router.navigate(['assinatura/pendencias']);
  }
}
