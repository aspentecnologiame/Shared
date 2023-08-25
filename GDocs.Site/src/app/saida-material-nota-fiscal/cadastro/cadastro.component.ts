import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { Router } from '@angular/router';
import { ExibicaoDeAlertaService, TipoAlertaEnum } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { SaidaMaterialNotaFiscalService } from '../saida-material-nota-fiscal.service';
import { SaidaMaterialNotaFiscal } from '../shared/models/SaidaMaterialNotaFiscal';
import { ItemSaidaMaterialNF } from '../shared/models/ItemSaidaMaterialNF';
import { DropDownModel } from 'src/app/shared/models/dropdown.model';
import { Fornecedor } from '../shared/models/fornecedor';

@Component({
  selector: 'app-cadastro',
  templateUrl: './cadastro.component.html',
  styleUrls: ['./cadastro.component.scss']
})
export class CadastroComponent implements OnInit {

  formValidationMaterialNfHelper: FormValidationHelper;
  formMaterialSaidaNf: FormGroup;
  tiposSaida: DropDownModel[];
  escolhaFornecedor: DropDownModel[];
  listaNatureza: DropDownModel[];
  listaModalidadeFrete: DropDownModel[];
  dataSource: ItemSaidaMaterialNF[] = [];

  @ViewChild('btnDataPicker') botaoDatePicker: ElementRef;

  constructor(
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly service: SaidaMaterialNotaFiscalService,
    public dialog: MatDialog,
  ) { }

  ngOnInit() {
    this.formInit()
  }


  formInit() {

    this.escolhaFornecedorDropInit();
    this.tiposDeSaidaDropDownInit();
    this.listarNaturezaDropDownInit();
    this.listarModalidadeFreteDropDownInit();


    this.formValidationMaterialNfHelper = new FormValidationHelper({
      tiposaida: "Tipos de saída",
      setor: "Sigla do setor",
      origem: 'Origem',
      destino: "Destino",
      dataPrevista: "Previsão de retorno",
      dataDeSaida: "Data de saída",
      motivo: "Motivo",
      documento: "CPF/CNPJ",
      codigoTotvs: "Código TOTVS",
      volume: "Volume total",
      peso: "Peso total",
      transportador: "Quem vai trasportar",
      endereco: "Endereço Fornecedor",
      bairro: "Bairro ",
      cep: "Cep",
      cidade: "Cidade",
      estado: "Estado",
      naturezaOperacional: "Natureza da operação",
      modalidadeFrete: "Modalidade do Frete",
    });




    this.formMaterialSaidaNf = this.formBuilder.group({

      tiposaida: new FormControl(null, [
        Validators.required,
      ]),
      setor: new FormControl(null, [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(3),

      ]),
      origem: new FormControl(null, [
        Validators.required,
        Validators.maxLength(25),
      ]),
      destino: new FormControl(null, [
        Validators.required,
        Validators.maxLength(60),
      ]),
      dataDeSaida: new FormControl(null, [
        Validators.required,
      ]),
      dataPrevista: new FormControl(null, [
        Validators.required,
      ]),
      transportador: new FormControl(null, [
        Validators.maxLength(35),
        Validators.required,
      ]),
      documento: new FormControl(null, [
        Validators.maxLength(14),
        Validators.minLength(11),
        Validators.required,
      ]),
      codigoTotvs: new FormControl(null, [
        Validators.required,
        Validators.maxLength(25),
      ]),
      endereco: new FormControl(null, [
        Validators.maxLength(200),
      ]),
      bairro: new FormControl(null, [
        Validators.maxLength(100),
      ]),
      cep: new FormControl(null, [
        Validators.maxLength(100),
      ]),
      cidade: new FormControl(null, [
        Validators.maxLength(100),
      ]),
      estado: new FormControl(null, [
        Validators.maxLength(2),
      ]),
      naturezaOperacional: new FormControl(null, [
        Validators.required,
      ]),
      modalidadeFrete: new FormControl(null, [
        Validators.required,
      ]),
      volume: new FormControl(null, [
        Validators.maxLength(25),
        Validators.required,
      ]),
      peso: new FormControl(null, [
        Validators.maxLength(25),
        Validators.required,
      ]),
      escolhaFornecedor: new FormControl(0, [
        Validators.required,
      ]),
      motivo: new FormControl(null, [
        Validators.required,
        Validators.maxLength(255),
      ]),

    })

  }

  listarNaturezaDropDownInit() {
    this.service.listarNaturezaOperacao().subscribe((naturezaOperacional) => {
      this.listaNatureza = naturezaOperacional;
    })

  }

  listarModalidadeFreteDropDownInit() {
    this.service.listarModalidadeFrete().subscribe((modalidadeFrete) => {
      this.listaModalidadeFrete = modalidadeFrete;
    })

  }

  mostrarFornecedor(): boolean {
    return this.formMaterialSaidaNf.value.escolhaFornecedor == 0
  }


  private tiposDeSaidaDropDownInit() {
    this.service.listarTipoSaidaMaterialNF().subscribe((tiposSaida) => {
      this.tiposSaida = tiposSaida;
    });
  }

  private escolhaFornecedorDropInit() {
    this.service.listarEscolhaFornecedor().subscribe((listarEscolhaFornecedor) => {
      this.escolhaFornecedor = listarEscolhaFornecedor;
    });
  }

  private alertCadastradoSucesso() {
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: 'A solicitação saída material com nota fiscal foi cadastrada com sucesso !',
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    }).then(() => {
      this.router.navigate(['SaidaMaterialNotaFiscal/consulta/']);
    });
  }


  ClickTipoDeSaida() {
    this.listaNatureza = this.listaNatureza.filter(x => x.id != 1)
    if (this.formMaterialSaidaNf.value.tiposaida == 0) {
      this.formMaterialSaidaNf.get('dataPrevista').disable();
      this.formMaterialSaidaNf.get('dataPrevista').setValue(null);
      this.botaoDatePicker.nativeElement.classList.add('desabilitado');
    } else {
      this.formMaterialSaidaNf.get('dataPrevista').enable();
      this.botaoDatePicker.nativeElement.classList.remove('desabilitado');
    }
  }




  ClickEscolhaFornecedor() {
    if (this.formMaterialSaidaNf.value.escolhaFornecedor == 0) {
      this.formMaterialSaidaNf.get('codigoTotvs').setValidators(Validators.required);
      this.formMaterialSaidaNf.get('endereco').setValidators(Validators.maxLength(100));
      this.formMaterialSaidaNf.get('bairro').setValidators(Validators.maxLength(100));
      this.formMaterialSaidaNf.get('cep').setValidators(Validators.maxLength(100));
      this.formMaterialSaidaNf.get('cidade').setValidators(Validators.maxLength(100));
      this.formMaterialSaidaNf.get('estado').setValidators(Validators.maxLength(2));
      this.UpdateValidador();
    } else {
      this.formMaterialSaidaNf.get('endereco').setValidators(Validators.required);
      this.formMaterialSaidaNf.get('bairro').setValidators(Validators.required);
      this.formMaterialSaidaNf.get('cep').setValidators(Validators.required);
      this.formMaterialSaidaNf.get('cidade').setValidators(Validators.required);
      this.formMaterialSaidaNf.get('estado').setValidators([Validators.required, Validators.maxLength(2)]);
      this.formMaterialSaidaNf.get('codigoTotvs').reset();
      this.formMaterialSaidaNf.get('codigoTotvs').setValidators(Validators.maxLength(100));
      this.formMaterialSaidaNf.get('codigoTotvs').updateValueAndValidity();
    }

    this.ValidadorCpfCNPJ();
  }

  UpdateValidador() {
    this.formMaterialSaidaNf.get('codigoTotvs').updateValueAndValidity();
    this.formMaterialSaidaNf.get('endereco').updateValueAndValidity();
    this.formMaterialSaidaNf.get('bairro').updateValueAndValidity();
    this.formMaterialSaidaNf.get('cep').updateValueAndValidity();
    this.formMaterialSaidaNf.get('cidade').updateValueAndValidity();
    this.formMaterialSaidaNf.get('estado').updateValueAndValidity();
  }

  CodigoTotvsOnblur(){
    this.ValidadorCpfCNPJ();
  }

  ValidadorCpfCNPJ(){
    const campoCpfCnpj = this.formMaterialSaidaNf.get('codigoTotvs').value ;
    if(campoCpfCnpj == null || campoCpfCnpj == undefined || campoCpfCnpj == '')
    this.formMaterialSaidaNf.get('documento').setValidators([Validators.maxLength(14), Validators.minLength(11),Validators.required]);
    else
    this.formMaterialSaidaNf.get('documento').setValidators([Validators.maxLength(14), Validators.minLength(11)]);

       this.formMaterialSaidaNf.get('documento').updateValueAndValidity();
  }


  cpfcnpjmask():String {
     const value = this.formMaterialSaidaNf.get('documento').value as string;

     if(value == null || value == undefined)
        return ''

    if(value.length < 12){
      return '000.000.000-000'
    }
     else{
       return '00.000.000/0000-00'
     }
  }

  gravarFormDados(): void {
    if (!this.validarFormulario()) {
      return;
    }
    const NovaSaidaMaterialNotaFiscal = new SaidaMaterialNotaFiscal();
    NovaSaidaMaterialNotaFiscal.flgRetorno = this.formMaterialSaidaNf.controls['tiposaida'].value;
    NovaSaidaMaterialNotaFiscal.setorResponsavel = this.formMaterialSaidaNf.controls['setor'].value;
    NovaSaidaMaterialNotaFiscal.origem = this.formMaterialSaidaNf.controls['origem'].value;
    NovaSaidaMaterialNotaFiscal.destino = this.formMaterialSaidaNf.controls['destino'].value;
    NovaSaidaMaterialNotaFiscal.retorno = this.formMaterialSaidaNf.controls['dataPrevista'].value as Date;
    NovaSaidaMaterialNotaFiscal.saida = this.formMaterialSaidaNf.controls['dataDeSaida'].value as Date;
    NovaSaidaMaterialNotaFiscal.motivo = this.formMaterialSaidaNf.controls['motivo'].value;
    NovaSaidaMaterialNotaFiscal.documento = this.formMaterialSaidaNf.controls['documento'].value;
    NovaSaidaMaterialNotaFiscal.volume = this.formMaterialSaidaNf.controls['volume'].value;
    NovaSaidaMaterialNotaFiscal.peso = this.formMaterialSaidaNf.controls['peso'].value;
    NovaSaidaMaterialNotaFiscal.transportador = this.formMaterialSaidaNf.controls['transportador'].value;
    NovaSaidaMaterialNotaFiscal.naturezaOperacional = this.formMaterialSaidaNf.controls['naturezaOperacional'].value;
    NovaSaidaMaterialNotaFiscal.modalidadeFrete = this.formMaterialSaidaNf.controls['modalidadeFrete'].value;

    NovaSaidaMaterialNotaFiscal.itemMaterialNf = this.dataSource;

    if (this.formMaterialSaidaNf.value.escolhaFornecedor == 0) {
      NovaSaidaMaterialNotaFiscal.codigoTotvs = this.formMaterialSaidaNf.controls['codigoTotvs'].value;

    } else {
      NovaSaidaMaterialNotaFiscal.fornecedor = new Fornecedor();
      NovaSaidaMaterialNotaFiscal.fornecedor.bairro = this.formMaterialSaidaNf.controls['bairro'].value;
      NovaSaidaMaterialNotaFiscal.fornecedor.endereco = this.formMaterialSaidaNf.controls['endereco'].value;
      NovaSaidaMaterialNotaFiscal.fornecedor.cidade = this.formMaterialSaidaNf.controls['cidade'].value;
      NovaSaidaMaterialNotaFiscal.fornecedor.cep = this.formMaterialSaidaNf.controls['cep'].value;
      NovaSaidaMaterialNotaFiscal.fornecedor.estado = this.formMaterialSaidaNf.controls['estado'].value;
    }

    this.service.cadastrarMaterialSaidaNotaFiscal(NovaSaidaMaterialNotaFiscal)
      .subscribe(result => {
        if (result != undefined)
          this.alertCadastradoSucesso();
      })
  }


  private validarFormulario(): boolean {
    this.ValidadorCpfCNPJ();
    this.validarItemFormulario();

    const dataPrevista = this.formMaterialSaidaNf.value.dataPrevista as Date;
    const dataSaida = this.formMaterialSaidaNf.value.dataDeSaida as Date;
    const dataAtual = new Date();
    dataAtual.setDate(dataAtual.getDate() - 1);
    const errorList: Array<string> = this.formValidationMaterialNfHelper.getFormValidationErrorsMessages(this.formMaterialSaidaNf.controls);
    const msgInvalidaCombinacao = 'O tipo de saída e natureza da operação selecionados são incompatíveis. Por favor selecione outro tipo ou outra natureza.';



    if (this.formMaterialSaidaNf.controls['tiposaida'].value == 1 &&
      (this.formMaterialSaidaNf.controls['naturezaOperacional'].value == 4 ||
        this.formMaterialSaidaNf.controls['naturezaOperacional'].value == 5 ||
        this.formMaterialSaidaNf.controls['naturezaOperacional'].value == 6)) {
      errorList.push(msgInvalidaCombinacao);
    }

    if (this.formMaterialSaidaNf.controls['tiposaida'].value == 0 &&
      (this.formMaterialSaidaNf.controls['naturezaOperacional'].value == 2 ||
        this.formMaterialSaidaNf.controls['naturezaOperacional'].value == 3)) {
      errorList.push(msgInvalidaCombinacao);
    }

    if (dataPrevista != null && dataPrevista < dataAtual) {
      errorList.push('A Previsão de retorno não pode ser menor que a data atual.');
    }

    if (dataSaida != null && dataSaida < dataAtual) {
      errorList.push('A data de saída não pode ser menor que a data atual.');
    }

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    if (this.dataSource.length == 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', ' Obrigatório adicionar ao menos um material.');
      return false;
    }

    return true;




  }

  private validarItemFormulario() {
    this.formMaterialSaidaNf.get('tiposaida').setValue(this.formMaterialSaidaNf.value.tiposaida === null ? null : this.formMaterialSaidaNf.value.tiposaida);
    this.formMaterialSaidaNf.get('setor').setValue(this.formMaterialSaidaNf.value.setor === null ? null : this.formMaterialSaidaNf.value.setor);
    this.formMaterialSaidaNf.get('origem').setValue(this.formMaterialSaidaNf.value.origem === null ? null : this.formMaterialSaidaNf.value.origem);
    this.formMaterialSaidaNf.get('destino').setValue(this.formMaterialSaidaNf.value.destino === null ? null : this.formMaterialSaidaNf.value.destino);
    this.formMaterialSaidaNf.get('dataPrevista').setValue(this.formMaterialSaidaNf.value.dataPrevista === null ? null : this.formMaterialSaidaNf.value.dataPrevista);
    this.formMaterialSaidaNf.get('motivo').setValue(this.formMaterialSaidaNf.value.motivo === null ? null : this.formMaterialSaidaNf.value.motivo);
    this.formMaterialSaidaNf.get('documento').setValue(this.formMaterialSaidaNf.value.documento === null ? null : this.formMaterialSaidaNf.value.documento);
    this.formMaterialSaidaNf.get('volume').setValue(this.formMaterialSaidaNf.value.volume === null ? null : this.formMaterialSaidaNf.value.volume);
    this.formMaterialSaidaNf.get('peso').setValue(this.formMaterialSaidaNf.value.peso === null ? null : this.formMaterialSaidaNf.value.peso);
    this.formMaterialSaidaNf.get('transportador').setValue(this.formMaterialSaidaNf.value.transportador === null ? null : this.formMaterialSaidaNf.value.transportador);
    this.formMaterialSaidaNf.get('naturezaOperacional').setValue(this.formMaterialSaidaNf.value.naturezaOperacional === null ? null : this.formMaterialSaidaNf.value.naturezaOperacional);
    this.formMaterialSaidaNf.get('modalidadeFrete').setValue(this.formMaterialSaidaNf.value.modalidadeFrete === null ? null : this.formMaterialSaidaNf.value.modalidadeFrete);
  }

  AtualizarDataSourceItemNF(valor: ItemSaidaMaterialNF[]) {
    this.dataSource = valor;
  }


  cancelar(): void {
    this.exibicaoDeAlertaService
      .exibirMensagemAviso('Atenção!', 'Operação cancelada.')
      .then(() => {
        this.router.navigate(['SaidaMaterialNotaFiscal/consulta']);
      });
  }

}
