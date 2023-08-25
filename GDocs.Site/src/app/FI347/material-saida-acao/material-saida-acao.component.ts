import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase, ExibicaoDeAlertaService } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { ItemSaidaMateialModel } from '../shared/models/item-saida-material.model';
import { RegistroSaidaMaterial } from '../shared/models/registro-saida-material';
import { SaidaMaterialModel } from '../shared/models/saida-material.model';
import { StatusAcaoMaterial } from '../shared/enums/status-acao-materia';
import { DatePipe } from '@angular/common';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';
@Component({
  selector: 'app-material-saida-acao',
  templateUrl: './material-saida-acao.component.html',
  styleUrls: ['./material-saida-acao.component.scss']
})
export class MaterialSaidaAcaoComponent extends ComponentFormBase implements OnInit {
  public get dialogRef(): MatDialogRef<any> {
    return this._dialogRef;
  }
  public set dialogRef(value: MatDialogRef<any>) {
    this._dialogRef = value;
  }

  form: FormGroup;
  formValidationHelper: FormValidationHelper;
  SaidaMaterial: SaidaMaterialModel;
  acao: StatusAcaoMaterial;
  nameCampoData: string = "Data de retorno";
  nameCampoHora: string = "Horário da Saída";
  nameCampoObservacao: string = "Observação";
  RegistroSaida: RegistroSaidaMaterial;
  dataSource: ItemSaidaMateialModel[] = [];
  horarioPattern = '^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$';
  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly datePipe: DatePipe,
    private _dialogRef: MatDialogRef<any>,
    private eventMiddleService: EventMiddleService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {
    this.acao = this.data.tipoAcao;
    this.FormularioInitAcao();
    this.ExpirouSessaoFecharModalAcao();

    if (this.data.editarItem !== undefined) {
      this.PreencherFormularioEdicao()
      this.SaidaMaterial = this.data.editarItem;
      this.acao = this.data.tipoAcao;
    }

  }

  ExpirouSessaoFecharModalAcao(){
    this.eventMiddleService.getEvent()
    .subscribe(e => {
      if(e != null && e != undefined){
        this._dialogRef.close();
      }
    });
  }

  private FormularioInitAcao() {


    this.form = this.formBuilder.group({
      responsavel: [{ value: null, disabled: true }, [Validators.maxLength(20)]],
      origem: [{ value: null, disabled: true }, [Validators.maxLength(20)]],
      destino: [{ value: null, disabled: true }, Validators.maxLength(10)],
      conferente: [null, [Validators.required, Validators.maxLength(20)]],
      dataSaida: [null, Validators.required],
      horaSaida: [null, [Validators.required, Validators.pattern(this.horarioPattern)]],
      nomePortador: [null, [Validators.required, Validators.maxLength(20)]],
      setorEmpresaReposavel: [null, [Validators.required, Validators.maxLength(20)]],
      observacao: [null, [Validators.maxLength(200)]],
    });

    this.ConfiguracaoPorAcao()



    this.formValidationHelper = new FormValidationHelper({
      responsavel: 'Responsável',
      origem: "Origem",
      destino: "Destino",
      conferente: "Conferente SCP",
      dataSaida: this.nameCampoData,
      horaSaida: this.nameCampoHora,
      nomePortador: "Nome do portador",
      setorEmpresaReposavel: "Setor/empresa Responsável",
      observacao: this.nameCampoObservacao,
    });
  }


  ConfiguracaoPorAcao() {
    if (this.acao == StatusAcaoMaterial.SolicitacaoProrrogacao.valueOf() ||
      this.acao == StatusAcaoMaterial.SolicitacaoBaixaMaterialSemRetorno.valueOf()) {

      this.nameCampoObservacao = "Justificativa";
      this.form.get('conferente').setValidators(Validators.maxLength(20))
      this.form.get('horaSaida').setValidators(Validators.maxLength(5))
      this.form.get('nomePortador').setValidators(Validators.maxLength(20))
      this.form.get('setorEmpresaReposavel').setValidators(Validators.maxLength(20))
      this.form.get('observacao').setValidators([Validators.required, Validators.maxLength(200)])
    }

    if (this.acao == StatusAcaoMaterial.SolicitacaoBaixaMaterialSemRetorno.valueOf()) {
      this.form.get('dataSaida').setValidators(Validators.nullValidator)
    }

    if (this.acao == StatusAcaoMaterial.RegistroSaida.valueOf()) {
      this.nameCampoData = "Data de saída";
    }

    if (this.acao == StatusAcaoMaterial.SolicitacaoProrrogacao.valueOf()) {
      this.nameCampoData = "Data prorrogação";
    }

    if (this.acao == StatusAcaoMaterial.RegistroRetorno.valueOf()) {
      this.nameCampoHora = "Horário de retorno";
    }
  }

  private PreencherFormularioEdicao() {
    this.dataSource = this.data.editarItem.itemMaterial;
    this.form.get('responsavel').setValue(this.data.editarItem.nomeResponsavel)
    this.form.get('origem').setValue(this.data.editarItem.origem)
    this.form.get('destino').setValue(this.data.editarItem.destino)
  }



  confirmarModal() {
    if (!this.validarFormulario()) {
      return;
    }


    const SaidaDeMaterial = this.RegistroSaida = {
      solicitacaoMaterialAcaoItemId: null,
      idSaidaMaterialTipoAcao: this.acao,
      idSolicitacaoSaidaMaterial: this.data.editarItem.id,
      portador: this.form.controls['nomePortador'].value,
      conferente: this.form.controls['conferente'].value,
      setorEmpresa: this.form.controls['setorEmpresaReposavel'].value,
      observacao: this.form.controls['observacao'].value,
      dataAcao: this.formataData(),
      acaoItems: this.formtarItemMaterial()
    };
    this.dialogRef.close(SaidaDeMaterial);
  }

  MostarDataMaterial() {
    if (this.acao == StatusAcaoMaterial.SolicitacaoBaixaMaterialSemRetorno.valueOf())
      return false;

    return true;
  }

  MostraCampo(): boolean {
    if (this.acao == StatusAcaoMaterial.SolicitacaoProrrogacao.valueOf() ||
      this.acao == StatusAcaoMaterial.SolicitacaoBaixaMaterialSemRetorno.valueOf()) {
      return false;
    }
    return true;
  }


  EhSolicitacaoProrogacao(): boolean {
    if (this.acao == StatusAcaoMaterial.SolicitacaoProrrogacao.valueOf()) {
      return true;
    }
    return false;
  }


  PermitirBaixaSemRetornoERetorno(): boolean {
    if (this.acao == StatusAcaoMaterial.SolicitacaoBaixaMaterialSemRetorno.valueOf() ||
      this.acao == StatusAcaoMaterial.RegistroRetorno.valueOf()) {
      return true;
    }
    return false;
  }


  PermirtirSaidaRetornoProrrogacao(): boolean {
    if (this.acao == StatusAcaoMaterial.RegistroSaida.valueOf() ||
      this.acao == StatusAcaoMaterial.RegistroRetorno.valueOf() ||
      this.acao == StatusAcaoMaterial.SolicitacaoProrrogacao.valueOf()) {
      return true;
    }
    return false;
  }

  PermirtirSaidaOuRetorno(): boolean {
    if (this.acao == StatusAcaoMaterial.RegistroSaida.valueOf() ||
      this.acao == StatusAcaoMaterial.RegistroRetorno.valueOf()) {
      return true;
    }
    return false;
  }



  formtarItemMaterial(): ItemSaidaMateialModel[] {
    if (this.PermitirBaixaSemRetornoERetorno()) {
      return this.dataSource.filter(x => x.check != undefined && x.check)
    }
    return this.dataSource;
  }

  formataData(): string {
    if (this.form.controls['horaSaida'].value !== undefined && this.form.controls['horaSaida'].value !== null) {
      return `${this.form.controls['dataSaida'].value.toLocaleDateString('en-US')} ${this.form.controls['horaSaida'].value}`;
    }

    if (this.form.controls['dataSaida'].value == null || this.form.controls['dataSaida'].value == undefined) {
      return null;
    }
    return `${this.form.controls['dataSaida'].value.toLocaleDateString('en-US')}`;
  }


  cancelarModal() {
    this.dialogRef.close();
  }

  private validarFormulario(): boolean {
    this.form.get('conferente').setValue(this.form.value.conferente === null ? null : this.form.value.conferente);
    this.form.get('dataSaida').setValue(this.form.value.dataSaida === null ? null : this.form.value.dataSaida);
    this.form.get('horaSaida').setValue(this.form.value.horaSaida === null ? null : this.form.value.horaSaida);
    this.form.get('nomePortador').setValue(this.form.value.nomePortador === null ? null : this.form.value.nomePortador);
    this.form.get('setorEmpresaReposavel').setValue(this.form.value.setorEmpresaReposavel === null ? null : this.form.value.setorEmpresaReposavel);
    this.form.get('observacao').setValue(this.form.value.observacao === null ? null : this.form.value.observacao);

    const errorList: Array<string> = this.ValidarData();

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    if (this.PermitirBaixaSemRetornoERetorno()) {
      if (this.dataSource.filter(x => x.check != undefined && x.check).length == 0) {
        this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Selecione ao menos 1 material.');
        return false;
      }
    }

    return true;
  }

  private ValidarData() {
    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.form.controls);
    const dataAcao = this.form.value.dataSaida as Date;
    const dataRetono = this.data.editarItem.retorno;
    const dataAcaoAtual = new Date();

    if (this.PermirtirSaidaOuRetorno()) {
      if (dataAcao > dataAcaoAtual) {
        errorList.push(`${this.nameCampoData} não pode ser maior que a data atual.`);
      }
    }

    if (this.acao == StatusAcaoMaterial.SolicitacaoProrrogacao.valueOf() && dataAcao != undefined) {
      if (dataRetono > dataAcao.toISOString()) {
        errorList.push(`${this.nameCampoData} deve ser maior que a data retorno prevista ${this.datePipe.transform(dataRetono, 'dd/MM/yyyy')}.`);
      }
    }
    return errorList;
  }


}


