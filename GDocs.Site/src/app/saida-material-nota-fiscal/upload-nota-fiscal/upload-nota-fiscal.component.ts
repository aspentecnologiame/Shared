import { SaidaMaterialNotaFiscal } from './../shared/models/SaidaMaterialNotaFiscal';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ComponentFormBase, ExibicaoDeAlertaService } from 'src/app/core';
import { SaidaMaterialNotaFiscalService } from '../saida-material-nota-fiscal.service';
import { ItemSaidaMaterialNF } from '../shared/models/ItemSaidaMaterialNF';
import { Subject } from 'rxjs';
import { SaidaMaterialArquivoModel } from '../shared/models/SaidaMaterialArquivoModel';
import { FormValidationHelper } from 'src/app/helpers';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { TipoAnexo } from '../shared/enums/TipoAnexo';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { MatDialog } from '@angular/material';
import { StatusAcaoMaterialNotaFiscal } from '../shared/enums/StatusAcaoMaterialNotaFiscal';
import { StatusSaidaMaterialNF } from '../shared/enums/StatusSaidaMaterialNF';


@Component({
  selector: 'app-upload-nota-fiscal',
  templateUrl: './upload-nota-fiscal.component.html'
})
export class UploadNotaFiscalComponent extends ComponentFormBase implements OnInit {

  saidaMaterialNf:SaidaMaterialNotaFiscal;
  dataSourceItemNf: ItemSaidaMaterialNF[] = [];
  listarArquivosChangingValue: Subject<SaidaMaterialArquivoModel[]> = new Subject();
  listaArquivoSaidaMaterialUpload: SaidaMaterialArquivoModel[] = [];
  indexGroupTab: number = 0
  formValidationUpload: FormValidationHelper;
  formUploadNf: FormGroup;


  constructor(
    private readonly activedRoute: ActivatedRoute,
    private readonly service: SaidaMaterialNotaFiscalService,
    private readonly formBuilder: FormBuilder,
    private readonly exibirAlertaService: ExibicaoDeAlertaService,
    private readonly router: Router,
    public dialog: MatDialog,

  ) {
    super();
  }


  ngOnInit() {

    super.ngOnInit();
    const smnf_idt = this.activedRoute.snapshot.paramMap.get('smnfId');

    if (smnf_idt !== null && smnf_idt !== undefined && smnf_idt !== '' && !isNaN(Number(smnf_idt.toString()))) {
       this.popularInformacao(smnf_idt);
    }

    super.ngOnInit();
    this.formularioUploadNfInit()
  }

  desabilitarRquired(): void {
    if (this.verificaEhPendenteNfRetorno()) {
      this.formUploadNf.get('numeroNf').clearValidators();
    }

  }

  async formularioUploadNfInit(){
    this.formValidationUpload = new FormValidationHelper({
      numeroNf: "Número da nota fiscal"
    });

    this.formUploadNf = this.formBuilder.group({
      numeroNf: new FormControl(null, [
        Validators.required,
      ])
    })

  }

  ConfigColuna():number{
    return StatusAcaoMaterialNotaFiscal.TodasColunas.valueOf()
  }


  uploadCompletoAdicionarNaListaDeDocumentosPreview(assinaturaUploadResponseModel: SaidaMaterialArquivoModel): void {
     this.listaArquivoSaidaMaterialUpload.push(assinaturaUploadResponseModel);
     this.listarArquivosChangingValue.next(this.listaArquivoSaidaMaterialUpload);
  }

    popularInformacao(smnf_idt:any){
       this.service.ObterMaterialSaidaNfPorId(smnf_idt)
    .toPromise()
    .then((result) =>
      {
        this.saidaMaterialNf = result;
        this.dataSourceItemNf = this.saidaMaterialNf.itemMaterialNf;
        this.indexGroupTab = (this.saidaMaterialNf.statusId != StatusSaidaMaterialNF.PendenteNfSaida)?1:0
        this.desabilitarRquired();
        this.validarDocumento()
      })

  }

  validarDocumento(){
    if(this.saidaMaterialNf.statusId != StatusSaidaMaterialNF.PendenteNfSaida &&
    this.saidaMaterialNf.statusId != StatusSaidaMaterialNF.SaidaPendente &&
    this.saidaMaterialNf.statusId != StatusSaidaMaterialNF.PendenteNfRetorno)
      {
        this.exibirAlertaService
        .exibirMensagemAviso('Atenção!', 'Essa solicitação não está mais pendente.')
        .then(() => {
          this.router.navigate(['SaidaMaterialNotaFiscal/consulta']);
        });
      }
  }

  podeEditar():boolean{
    return this.saidaMaterialNf.numero != 0  &&  this.saidaMaterialNf.numero != undefined  &&  this.saidaMaterialNf.numero != null
  }

  mostraJustificativa():boolean{
    return (this.saidaMaterialNf.statusId == StatusSaidaMaterialNF.SaidaPendente)
  }

  verificaEhPendenteNfRetorno():boolean{
    return (this.saidaMaterialNf != undefined &&  this.saidaMaterialNf.statusId == StatusSaidaMaterialNF.PendenteNfRetorno)
  }

  salvarAnexo(){
    if (!this.validarFormulario())
      return;
    if(this.mostraJustificativa())
      this.modalJustificativa();
    else
     this.enviarRequisicao();

  }

  enviarRequisicao(){
    this.service.cadastraArquivosUpload(this.listaArquivoSaidaMaterialUpload)
    .subscribe(
      () => {
        this.exibirAlertaService
        .exibirMensagemSucesso('Sucesso!', 'Arquivo(s) adicionado(s) com sucesso!')
        .then(() => this.navegarParaConsultaMaterialNf());
    });
  }

  modalJustificativa(){
    const dialogRef = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '285px',
      disableClose: true,
      data: {
        titulo: 'Motivo da troca nota fiscal',
        confirmar: 'Confirmar',
        nomeCampo:'Motivo',
        maxLength:255
      }
    });

    dialogRef.afterClosed().subscribe(retorno => {
      if (retorno === undefined || retorno.justificativa === null) {
        return;
      }
     this.listaArquivoSaidaMaterialUpload.forEach(a => a.motivo = retorno.justificativa);
     this.enviarRequisicao();
    });

  }

  navegarParaConsultaMaterialNf(): void {
      this.router.navigate(['SaidaMaterialNotaFiscal/consulta/']);
  }



  validarFormulario():boolean{
    this.formUploadNf.get('numeroNf').setValue(this.formUploadNf.value.numeroNf === null ? null : this.formUploadNf.value.numeroNf);
    const errorList: Array<string> = this.formValidationUpload.getFormValidationErrorsMessages(this.formUploadNf.controls);

    if (this.listaArquivoSaidaMaterialUpload.length == 0 ) {
      errorList.push('É obrigatório inserir um anexo');
    }

    if (errorList.length > 0) {
      this.exibirAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    this.criarObjetoRequisao();
    return true;
  }

  cancelarOperacao(): void {
    this.exibirAlertaService
      .exibirMensagemAviso('Atenção!', 'Operação cancelada.')
      .then(() => {
        this.router.navigate(['SaidaMaterialNotaFiscal/consulta']);
      });
  }

  criarObjetoRequisao()
  {
    this.listaArquivoSaidaMaterialUpload.forEach(a => a.saidaMaterialNfId = this.saidaMaterialNf.id);

    if (!this.verificaEhPendenteNfRetorno()) {
      this.listaArquivoSaidaMaterialUpload.forEach(b => b.numero = this.formUploadNf.value.numeroNf);
    }

    this.listaArquivoSaidaMaterialUpload.forEach(b => b.tipoNatureza = this.TipoDeNatureza());
  }

  TipoDeNatureza():number{
  if(this.verificaEhPendenteNfRetorno())
      return TipoAnexo.Retorno.valueOf();
    else
    return TipoAnexo.Saida.valueOf();
  }

}
