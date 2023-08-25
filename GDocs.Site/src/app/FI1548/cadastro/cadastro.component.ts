import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog, MatStepper } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { AssinaturaAssinadoresComponent } from 'src/app/assinatura/assinatura-cadastro/assinatura-assinadores/assinatura-assinadores.component';
import { AssinaturaStep } from 'src/app/assinatura/enums/assinatura-step-enum';
import { AssinaturaInformacoesModel, AssinaturaUploadResponseModel } from 'src/app/assinatura/models';
import { PassoAssinaturaUsuariosAdAdcionadosPaiModel } from 'src/app/assinatura/models/passo-assinatura-usuarios-ad-adicionados-pai.model';
import { PdfPreviewComponent } from 'src/app/shared/pdf-preview/pdf-preview.component';
import { ComponentFormBase, ExibicaoDeAlertaService } from '../../core';
import { Fi1548Service } from '../FI1548.service';
import { DocumentoModel } from '../shared';
import { DocumentoEhAssinaturaPassosModel } from '../shared/models/documento-eh-assinatura-passos';
import { CadastroFormularioComponent } from './cadastro-formulario/cadastro-formulario.component';


@Component({
  selector: 'app-cadastro',
  templateUrl: './cadastro.component.html',
  styleUrls: ['./cadastro.component.scss'],
})
export class CadastroComponent extends ComponentFormBase implements OnInit {

  documentoAssinaturaPassosModel:DocumentoEhAssinaturaPassosModel = new DocumentoEhAssinaturaPassosModel();
  passoAssinaturaUsuariosAdAdcionadosPai: PassoAssinaturaUsuariosAdAdcionadosPaiModel = new PassoAssinaturaUsuariosAdAdcionadosPaiModel();
  assinaturaInformacoes: AssinaturaInformacoesModel = new AssinaturaInformacoesModel();
  novoDocumento: DocumentoModel = new DocumentoModel();
  listaDocumentoUpload: AssinaturaUploadResponseModel[] = [];
  atualizarPasso:boolean = true;
  errorsFormStatusPagamento: string[] = [];

  @ViewChild('stepper') private readonly stepper: MatStepper;
  @ViewChild('assinaturaAssinadoresComponent') public componenteAssinadores: AssinaturaAssinadoresComponent;
  @ViewChild('cadastroFormularioComponent') public cadastroFormulario: CadastroFormularioComponent;
  @ViewChild('pdfViewUpload') public componentePdfViewUpload: PdfPreviewComponent;

  constructor(
    public dialog: MatDialog,
    private readonly service: Fi1548Service,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly activedRoute: ActivatedRoute,
    private readonly router: Router,
  ) {
    super();
  }

  async ngOnInit() {

    await this.edicaoBuscaPassos()
    super.ngOnInit();
  }

  async edicaoBuscaPassos(){
    const dfi_idt = this.activedRoute.snapshot.paramMap.get('dfi_idt');
    if (dfi_idt !== null && dfi_idt !== undefined && dfi_idt !== '' && dfi_idt != '0' && !isNaN(Number(dfi_idt.toString()))) {
       this.service.obterPassoEhUsuarioPorId(dfi_idt)
       .subscribe((result) =>{
        if(result != null )
           this.passoAssinaturaUsuariosAdAdcionadosPai.itens= result;
       });
    }
  }

  private validarStep1Informacoes(): boolean {
    this.cadastroFormulario.validarFormulario();
    if (this.errorsFormStatusPagamento.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', this.errorsFormStatusPagamento.join('<br />'));
      return false;
    }

    return true;
  }

  private validarStep2Assinadores(): boolean {
    if (this.passoAssinaturaUsuariosAdAdcionadosPai.itens.length > 0 &&
        this.passoAssinaturaUsuariosAdAdcionadosPai.itens.filter(passo => !passo.usuarios.length).length) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Todos os passos devem conter ao menos 1 aprovador.');
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
    if ((this.stepper.selectedIndex === AssinaturaStep.Step1Cabecalho && !this.validarStep1Informacoes() ) ||
        (this.stepper.selectedIndex === AssinaturaStep.Step2Assinatura && !this.validarStep2Assinadores())) {
        return;
    }
    if (this.stepper.selectedIndex === AssinaturaStep.Step1Cabecalho) {
      this.configuracaoStep2();
    }

    if(this.stepper.selectedIndex === AssinaturaStep.Step1Cabecalho) {
      this.cadastroFormulario.BuildFormulario()
    }

    if(this.stepper.selectedIndex === AssinaturaStep.Step2Assinatura){
      this.CadastraDocumento()
      return;
    }

    this.stepper.next();
  }


  private configuracaoStep2() {
    this.componenteAssinadores.atualizarConfiguracaoCertificadoDigital();
    this.componenteAssinadores.desabilitarAssinaturaDocumento();
    if(this.atualizarPasso){
      this.componenteAssinadores.carregarPassoPraEdicao();
      this.componenteAssinadores.incluirDirEhEsconderCheck();
    }
    this.atualizarPasso = false
  }

  CadastraDocumento(){

   this.service.cadastrarDocumento(this.novoDocumento)
      .subscribe(StatusPagamentos => {
         if(StatusPagamentos != null){
           this.EnviarParaAssinatura(StatusPagamentos);
         }
     },
     () =>{
      this.stepAnterior()
     });
  }

  private EnviarParaAssinatura(StatusPagamentos: DocumentoModel) {
    this.novoDocumento = StatusPagamentos;
    this.documentoAssinaturaPassosModel.documentoModel = StatusPagamentos;
    this.documentoAssinaturaPassosModel.assinaturaPassoModel = this.passoAssinaturaUsuariosAdAdcionadosPai;
    this.service.enviarParaAssinatura(this.documentoAssinaturaPassosModel)
      .subscribe(response => {
        if (response != null) {
          this.listaDocumentoUpload = response;
          this.stepper.next();
        }
      });
  }


  stepAnterior() {
    this.stepper.previous();
  }

  cancelarOperacao(){
    this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Operação cancelada.')
    .then(() => {
      this.router.navigate(['fi1548/consultar']);
    })
    .catch((result)=> {console.error(result)});
  }

}
