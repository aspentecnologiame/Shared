import { Component, OnInit, ViewChild } from '@angular/core';
import { MatStepper } from '@angular/material';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { PdfPreviewComponent } from '../../../app/shared/pdf-preview/pdf-preview.component';
import { ComponentFormBase, ExibicaoDeAlertaService } from '../../core';
import { AssinaturaService } from '../assinatura.service';
import { AssinaturaStep } from '../enums/assinatura-step-enum';
import { AssinaturaInformacoesModel, AssinaturaModel, AssinaturaUploadResponseModel, AssinaturaUsuarioAdModel, PassoAssinaturaUsuariosAdAdcionadosPaiModel } from '../models';
import { AssinaturaAssinadoresComponent } from './assinatura-assinadores/assinatura-assinadores.component';

@Component({
  selector: 'app-assinatura-cadastro',
  templateUrl: './assinatura-cadastro.component.html',
  styleUrls: ['./assinatura-cadastro.component.scss']
})
export class AssinaturaCadastroComponent extends ComponentFormBase implements OnInit {
  listaAssinaturaDocumentosUpload: AssinaturaUploadResponseModel[] = [];
  listarDocumentosChangingValue: Subject<AssinaturaUploadResponseModel[]> = new Subject();
  assinaturaInformacoes: AssinaturaInformacoesModel = new AssinaturaInformacoesModel();
  errorsAssinaturaInformacoes: string[] = [];
  assinaturaUsuarioAdModelValorAnterior: AssinaturaUsuarioAdModel;
  passoAssinaturaUsuariosAdAdcionadosPai: PassoAssinaturaUsuariosAdAdcionadosPaiModel = new PassoAssinaturaUsuariosAdAdcionadosPaiModel();

  @ViewChild('stepper') private readonly stepper: MatStepper;
  @ViewChild('assinaturaAssinadoresComponent') public componenteAssinadores: AssinaturaAssinadoresComponent;
  @ViewChild('pdfViewUpload') public componentePdfViewUpload: PdfPreviewComponent;

  constructor(
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly router: Router,
    private readonly assinaturaService: AssinaturaService) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
  }

  private validarStep3Upload(): boolean {
    if (this.listaAssinaturaDocumentosUpload.length === 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'É necessário fazer o upload de ao menos 1 arquivo.');
      return false;
    }

    return true;
  }

  private validarStep1Informacoes(): boolean {
    if (this.assinaturaInformacoes.numeroDocumento !== null && this.assinaturaInformacoes.numeroDocumento < 1) {
      this.errorsAssinaturaInformacoes.push('O campo Número do documento deve ser maior que 0 (zero).');
    }

    if (this.errorsAssinaturaInformacoes.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', this.errorsAssinaturaInformacoes.join('<br />'));
      return false;
    }

    return true;
  }

  private validarStep2Assinadores(): boolean {
    if (this.passoAssinaturaUsuariosAdAdcionadosPai.itens.length === 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'É necessário incluir ao menos 1 passo com aprovadores, mesmo que a DIR esteja selecionada.');
      return false;
    }

    if (this.passoAssinaturaUsuariosAdAdcionadosPai.itens.length > 0 &&
        this.passoAssinaturaUsuariosAdAdcionadosPai.itens.filter(passo => !passo.usuarios.length).length) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Todos os passos devem conter ao menos 1 aprovador.');
      return false;
    }

    if (this.assinaturaInformacoes.assinaturaDocumento &&
      !this.componenteAssinadores.desabilitarAssinaturaDocumento() &&
      (this.componenteAssinadores.matDataSource.data === null || this.componenteAssinadores.matDataSource.data === undefined || this.componenteAssinadores.matDataSource.data.length === 0)) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Necessário informar a assinatura no documento.');

      return false;
    }

    this.setarUsuarioComoAssinaturaDocumento();

    return true;
  }

  private setarUsuarioComoAssinaturaDocumento() {
    let listaVazia = false;
    if (this.componenteAssinadores.matDataSource === undefined ||
      this.componenteAssinadores.matDataSource === null ||
      this.componenteAssinadores.matDataSource.data === undefined ||
      this.componenteAssinadores.matDataSource.data === null) {
        listaVazia = true;
    }

    this.passoAssinaturaUsuariosAdAdcionadosPai.itens.forEach(passo => {
      passo.usuarios.forEach(usu => {
        if (!listaVazia && this.componenteAssinadores.matDataSource.data.filter(w => w.guid === usu.guid).length > 0) {
          usu.assinarFisicamente = true;
        } else {
          usu.assinarFisicamente = false;
        }
      });
    });

    this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir.forEach(passoDir => {
      passoDir.usuarios.forEach(usuDir => {
        if (!listaVazia && this.componenteAssinadores.matDataSource.data.filter(w => w.guid === usuDir.guid).length > 0) {
          usuDir.assinarFisicamente = true;
        } else {
          usuDir.assinarFisicamente = false;
        }
      });
    });
  }

  async proximoStep() {
    if ((this.stepper.selectedIndex === AssinaturaStep.Step1Cabecalho && !this.validarStep1Informacoes()) ||
        (this.stepper.selectedIndex === AssinaturaStep.Step2Assinatura && !this.validarStep2Assinadores())) {
        return;
    }

    if (this.stepper.selectedIndex === AssinaturaStep.Step1Cabecalho) {
      this.componenteAssinadores.atualizarConfiguracaoCertificadoDigital();
      this.componenteAssinadores.desabilitarAssinaturaDocumento();
    }

    if (this.stepper.selectedIndex === AssinaturaStep.Step2Assinatura &&
        this.assinaturaInformacoes.numeroDocumentoAutomatico) {
          this.gerarNumeracaoAutomatica(this.assinaturaInformacoes.categoriaId);
    }

    this.stepper.next();
  }

  async gerarNumeracaoAutomatica(categoriaId: number) {
    const numeracaoAutomaticaJaGerada = this.assinaturaInformacoes.listaNumeracaoAutomatica.find(w => w.categoriaId === categoriaId);
    if (numeracaoAutomaticaJaGerada === undefined ||
        numeracaoAutomaticaJaGerada === null) {
          await this.assinaturaService.gerarNumeracaoAutomatica(categoriaId)
          .toPromise()
          .then((response) => {
            this.assinaturaInformacoes.listaNumeracaoAutomatica.push({ categoriaId: categoriaId , numeroDocumento: response })
            this.assinaturaInformacoes.numeroDocumento = response;
          });
    } else {
      this.assinaturaInformacoes.numeroDocumento = numeracaoAutomaticaJaGerada.numeroDocumento;
    }
  }

  stepAnterior() {
    this.stepper.previous();
  }

  checarNecessidadeDeLimparUpload() {
    if (this.listaAssinaturaDocumentosUpload.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso(
        'Atenção!',
        `Essa alteração requer que seja feito o upload do documento novamente.`)
        .then(() => {
            this.listaAssinaturaDocumentosUpload = [];
            this.componentePdfViewUpload.removerTodosDocumentos();
        });
    }
  }

  salvarDocumentosInformacoesPessoasAssinatura(): void {
    if (!this.validarStep3Upload()) {
      return;
    }

    this.assinaturaService.gravar(new AssinaturaModel(this.listaAssinaturaDocumentosUpload, this.assinaturaInformacoes, this.passoAssinaturaUsuariosAdAdcionadosPai))
      .subscribe((assinaturaCriada) => {
        this.exibicaoDeAlertaService.exibirMensagemSucesso(
          'Sucesso!',
          `Arquivo(s) adicionado(s) para aprovação com sucesso! \n Número do documento: ${assinaturaCriada.informacoes.numeroDocumento}`)
          .then(() => {
          this.router.navigate(['assinatura/gerenciamento']);
        });
      });
  }

  uploadCompletoAdicionarNaListaDeDocumentosPreview(assinaturaUploadResponseModel: AssinaturaUploadResponseModel): void {
    this.listaAssinaturaDocumentosUpload.push(assinaturaUploadResponseModel);
    this.listarDocumentosChangingValue.next(this.listaAssinaturaDocumentosUpload);
  }

  cancelarOperacao(): void {
    this.exibicaoDeAlertaService
      .exibirMensagemAviso('Atenção!', 'Operação cancelada.')
      .then(() => {
        this.router.navigate(['/assinatura/gerenciamento']);
      });
  }

  retornarQtdeDeArquivosUpload(): number {
    return this.listaAssinaturaDocumentosUpload.length;
  }
}
