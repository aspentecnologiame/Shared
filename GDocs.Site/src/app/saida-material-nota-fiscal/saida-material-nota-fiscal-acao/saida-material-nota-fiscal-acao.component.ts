import { DatePipe } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase, ExibicaoDeAlertaService } from 'src/app/core';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';
import { FormValidationHelper } from 'src/app/helpers';
import { StatusAcaoMaterialNotaFiscal } from '../shared/enums/StatusAcaoMaterialNotaFiscal';
import { ItemSaidaMaterialNF } from '../shared/models/ItemSaidaMaterialNF';
import { SaidaMaterialNotaFiscal } from '../shared/models/SaidaMaterialNotaFiscal';
import { SaidaMaterialNotafiscalAcaoModel } from '../shared/models/SaidaMaterialNotafiscalAcaoModel';

@Component({
  selector: 'app-saida-material-nota-fiscal-acao',
  templateUrl: './saida-material-nota-fiscal-acao.component.html',
  styleUrls: ['./saida-material-nota-fiscal-acao.component.scss']
})
export class SaidaMaterialNotaFiscalAcaoComponent extends ComponentFormBase implements OnInit {

  public get dialogRef(): MatDialogRef<any> {
    return this._dialogRefNf;
  }
  public set dialogRef(value: MatDialogRef<any>) {
    this._dialogRefNf = value;
  }


  formSaidaMaterialNfAcao: FormGroup;
  formValidationHelperNf: FormValidationHelper;
  SaidaMaterial: SaidaMaterialNotaFiscal;
  acao: StatusAcaoMaterialNotaFiscal;
  saidaMaterialNfAcao: SaidaMaterialNotafiscalAcaoModel;
  dataSource: ItemSaidaMaterialNF[] = [];
  nameCampoData: string = "Data de retorno";
  nameCampoHora: string = "Horário da Saída";
  nameCampoObservacao: string = "Observação";
  horarioPattern = '^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$';
  constructor(
    private readonly formBuilderNF: FormBuilder,
    private readonly AlertaServiceNf: ExibicaoDeAlertaService,
    private readonly pipeDate: DatePipe,
    private _dialogRefNf: MatDialogRef<any>,
    private eventMiddleService: EventMiddleService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {
    this.acao = this.data.tipoAcao;
    this.FormularioInitAcao();
    this.ExpirouSessaoFecharModal()

    if (this.data.editarItem !== undefined) {
      this.PreencherFormValorPadrao()
      this.SaidaMaterial = this.data.editarItem;
    }

  }


  private FormularioInitAcao() {


    this.formSaidaMaterialNfAcao = this.formBuilderNF.group({
      solicitanteNf: [{ value: null, disabled: true }, [Validators.maxLength(20)]],
      origemNf: [{ value: null, disabled: true }, [Validators.maxLength(20)]],
      destinoNf: [{ value: null, disabled: true }, Validators.maxLength(10)],
      conferenteNf: [null, [Validators.required, Validators.maxLength(20)]],
      dataSaidaNf: [null, Validators.required],
      horaSaidaNf: [null, [Validators.required, Validators.pattern(this.horarioPattern)]],
      nomePortadorNf: [null, [Validators.required, Validators.maxLength(20)]],
      setorEmpresaReposavelNf: [null, [Validators.required, Validators.maxLength(20)]],
      observacaoNf: [null, [Validators.maxLength(200)]],
    });

    this.ConfigurarFormPorAcao()



    this.formValidationHelperNf = new FormValidationHelper({
      solicitanteNf: 'Responsável',
      origemNf: "Origem",
      destinoNf: "Destino",
      conferenteNf: "Conferente SCP",
      dataSaidaNf: this.nameCampoData,
      horaSaidaNf: this.nameCampoHora,
      nomePortadorNf: "Nome do portador",
      setorEmpresaReposavelNf: "Setor/empresa Responsável",
      observacaoNf: this.nameCampoObservacao,
    });
  }


  ConfigurarFormPorAcao() {
    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf.valueOf() ||
      this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoBaixaMaterialNFSemRetorno.valueOf()) {

      this.nameCampoObservacao = "Justificativa";
      this.formSaidaMaterialNfAcao.get('conferenteNf').setValidators(Validators.maxLength(20))
      this.formSaidaMaterialNfAcao.get('horaSaidaNf').setValidators(Validators.maxLength(5))
      this.formSaidaMaterialNfAcao.get('nomePortadorNf').setValidators(Validators.maxLength(20))
      this.formSaidaMaterialNfAcao.get('setorEmpresaReposavelNf').setValidators(Validators.maxLength(20))
      this.formSaidaMaterialNfAcao.get('observacaoNf').setValidators([Validators.required, Validators.maxLength(200)])
    }

    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoBaixaMaterialNFSemRetorno.valueOf()) {
      this.formSaidaMaterialNfAcao.get('dataSaidaNf').setValidators(Validators.nullValidator)
    }

    if (this.acao == StatusAcaoMaterialNotaFiscal.Saida.valueOf()) {
      this.nameCampoData = "Data de saída";
    }

    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf.valueOf()) {
      this.nameCampoData = "Data prorrogação";
    }

    if (this.acao == StatusAcaoMaterialNotaFiscal.Retorno.valueOf()) {
      this.nameCampoHora = "Horário de retorno";
    }
  }

  private PreencherFormValorPadrao() {
    this.dataSource = this.data.editarItem.itemMaterialNf;
    this.formSaidaMaterialNfAcao.get('solicitanteNf').setValue(this.data.editarItem.nomeAutor)
    this.formSaidaMaterialNfAcao.get('origemNf').setValue(this.data.editarItem.origem)
    this.formSaidaMaterialNfAcao.get('destinoNf').setValue(this.data.editarItem.destino)
  }



  confirmarModal() {
    if (!this.validarFormularioNf()) {
      return;
    }

    const SaidaDeMaterialAcao =  new SaidaMaterialNotafiscalAcaoModel();
    SaidaDeMaterialAcao.idSaidaMaterialNotaFiscal =this.data.editarItem.id;
    SaidaDeMaterialAcao.idSaidaMaterialNotaFiscalTipoAcao = this.acao;
    SaidaDeMaterialAcao.portador = this.formSaidaMaterialNfAcao.controls['nomePortadorNf'].value;
    SaidaDeMaterialAcao.conferente = this.formSaidaMaterialNfAcao.controls['conferenteNf'].value;
    SaidaDeMaterialAcao.setorEmpresa = this.formSaidaMaterialNfAcao.controls['setorEmpresaReposavelNf'].value;
    SaidaDeMaterialAcao.observacao = this.formSaidaMaterialNfAcao.controls['observacaoNf'].value;
    SaidaDeMaterialAcao.dataAcao = this.formataData();
    SaidaDeMaterialAcao.acaoItems =this.formtarItemMaterial();

    this.dialogRef.close(SaidaDeMaterialAcao);
  }

  MostarDataMaterial() {
    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoBaixaMaterialNFSemRetorno.valueOf())
      return false;

    return true;
  }

  MostraCampo(): boolean {
    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf.valueOf() ||
      this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoBaixaMaterialNFSemRetorno.valueOf()) {
      return false;
    }
    return true;
  }


  EhSolicitacaoProrogacao(): boolean {
    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf.valueOf()) {
      return true;
    }
    return false;
  }


  PermitirBaixaSemRetornoERetorno(): boolean {
    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoBaixaMaterialNFSemRetorno.valueOf() ||
      this.acao == StatusAcaoMaterialNotaFiscal.Retorno.valueOf()) {
      return true;
    }
    return false;
  }


  PermirtirSaidaRetornoProrrogacao(): boolean {
    if (this.acao == StatusAcaoMaterialNotaFiscal.Saida.valueOf() ||
      this.acao == StatusAcaoMaterialNotaFiscal.Retorno.valueOf() ||
      this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf.valueOf()) {
      return true;
    }
    return false;
  }

  PermirtirSaidaOuRetorno(): boolean {
    if (this.acao == StatusAcaoMaterialNotaFiscal.Saida.valueOf() ||
      this.acao == StatusAcaoMaterialNotaFiscal.Retorno.valueOf()) {
      return true;
    }
    return false;
  }


  formtarItemMaterial(): ItemSaidaMaterialNF[] {
    if (this.PermitirBaixaSemRetornoERetorno()) {
      return this.dataSource.filter(x => x.check != undefined && x.check)
    }
    return this.dataSource;
  }

  formataData(): string {
    if (this.formSaidaMaterialNfAcao.controls['horaSaidaNf'].value !== undefined && this.formSaidaMaterialNfAcao.controls['horaSaidaNf'].value !== null) {
      return `${this.formSaidaMaterialNfAcao.controls['dataSaidaNf'].value.toLocaleDateString('en-US')} ${this.formSaidaMaterialNfAcao.controls['horaSaidaNf'].value}`;
    }

    if (this.formSaidaMaterialNfAcao.controls['dataSaidaNf'].value == null || this.formSaidaMaterialNfAcao.controls['dataSaidaNf'].value == undefined) {
      return null;
    }
    return `${this.formSaidaMaterialNfAcao.controls['dataSaidaNf'].value.toLocaleDateString('en-US')}`;
  }


  cancelarModal() {
    this.dialogRef.close();
  }

  private validarFormularioNf(): boolean {
    this.formSaidaMaterialNfAcao.get('conferenteNf').setValue(this.formSaidaMaterialNfAcao.value.conferenteNf === null ? null : this.formSaidaMaterialNfAcao.value.conferenteNf);
    this.formSaidaMaterialNfAcao.get('dataSaidaNf').setValue(this.formSaidaMaterialNfAcao.value.dataSaidaNf === null ? null : this.formSaidaMaterialNfAcao.value.dataSaidaNf);
    this.formSaidaMaterialNfAcao.get('horaSaidaNf').setValue(this.formSaidaMaterialNfAcao.value.horaSaidaNf === null ? null : this.formSaidaMaterialNfAcao.value.horaSaidaNf);
    this.formSaidaMaterialNfAcao.get('nomePortadorNf').setValue(this.formSaidaMaterialNfAcao.value.nomePortadorNf === null ? null : this.formSaidaMaterialNfAcao.value.nomePortadorNf);
    this.formSaidaMaterialNfAcao.get('setorEmpresaReposavelNf').setValue(this.formSaidaMaterialNfAcao.value.setorEmpresaReposavelNf === null ? null : this.formSaidaMaterialNfAcao.value.setorEmpresaReposavelNf);
    this.formSaidaMaterialNfAcao.get('observacaoNf').setValue(this.formSaidaMaterialNfAcao.value.observacaoNf === null ? null : this.formSaidaMaterialNfAcao.value.observacaoNf);

    const errorList: Array<string> = this.ValidarData();

    if (errorList.length > 0) {
      this.AlertaServiceNf.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    if (this.PermitirBaixaSemRetornoERetorno()) {
      if (this.dataSource.filter(x => x.check != undefined && x.check).length == 0) {
        this.AlertaServiceNf.exibirMensagemAviso('Atenção!', 'Selecione ao menos 1 material.');
        return false;
      }
    }

    return true;
  }

  private ValidarData() {
    const errorList: Array<string> = this.formValidationHelperNf.getFormValidationErrorsMessages(this.formSaidaMaterialNfAcao.controls);
    const dataAcao = this.formSaidaMaterialNfAcao.value.dataSaidaNf as Date;
    const dataRetono = this.data.editarItem.retorno;
    const dataAcaoAtual = new Date();

    if (this.PermirtirSaidaOuRetorno()) {
      if (dataAcao > dataAcaoAtual) {
        errorList.push(`${this.nameCampoData} não pode ser maior que a data atual.`);
      }
    }

    if (this.acao == StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf.valueOf() && dataAcao != undefined) {
      if (dataRetono > dataAcao.toISOString()) {
        errorList.push(`${this.nameCampoData} deve ser maior que a data retorno prevista ${this.pipeDate.transform(dataRetono, 'dd/MM/yyyy')}.`);
      }
    }
    return errorList;
  }

  ExpirouSessaoFecharModal(){
    this.eventMiddleService.getEvent()
    .subscribe(expirou => {
      if(expirou != null && expirou != undefined){
        this.dialogRef.close();
      }
    });
  }

}
