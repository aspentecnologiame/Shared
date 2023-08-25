import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { AppConfig } from 'src/app/app.config';
import { ExibicaoDeAlertaService } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { CustomFormValidationsHelper } from 'src/app/helpers/custom-form-validations.helper';
import { IMoeda } from 'src/app/shared';
import { Fi1548Service } from '../../FI1548.service';
import { DocumentoModel, TipoPagamentoModel } from '../../shared';
import { StatusDocumento } from '../../shared/enums/status-documento';
import { TiposPagamento } from '../../shared/enums/tipos-pagamento';
import { FornecedorModel } from '../../shared/models/fornecedor.model';
import { ReferenciaSubstitutoModel } from '../../shared/models/referencia-substituto.model';

@Component({
  selector: 'app-cadastro-formulario',
  templateUrl: './cadastro-formulario.component.html',
  styleUrls: ['./cadastro-formulario.component.scss']
})
export class CadastroFormularioComponent  implements OnInit  {
  formDadosDocumento: FormGroup;
  listaMoedas: IMoeda[];
  listaStatus: TipoPagamentoModel[] = [];
  listaSubstitutos: ReferenciaSubstitutoModel[] = [];
  listaFornecedor: FornecedorModel[] = [];
  formValidationHelper: FormValidationHelper;
  descricaoMaximoColunas: number;
  descricaoMaximoLinhas: number;
  editarDocumento: DocumentoModel;

  @Input() documentoModel: DocumentoModel;
  @Output() documentoModelModelChange: EventEmitter<DocumentoModel> = new EventEmitter<DocumentoModel>();

  @Input() errors: string[];
  @Output() errorsChange = new EventEmitter<string[]>();


  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    public dialog: MatDialog,
    private readonly service: Fi1548Service,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly activedRoute: ActivatedRoute,
  ) {
  }


  async ngOnInit() {
    await this.formInit();
    this.iniciarListaStatus();
    this.iniciarFornecedores();
    this.EditarDocumento();
    this.listaMoedas = AppConfig.settings.moedas;
    this.descricaoMaximoColunas = AppConfig.settings.statusPagamento.maxColsDescricao;
    this.descricaoMaximoLinhas = AppConfig.settings.statusPagamento.maxRowsDescricao;
    this.iniciarSubstitutos();
  }


  private iniciarListaStatus() {
    this.service.listarTiposPagamento().subscribe((status) => {
      this.listaStatus = status;
    });
  }

  private iniciarFornecedores() {
    this.service.listarFornecedores().subscribe((fornecedor) => {
      this.listaFornecedor = fornecedor;
    });
  }

  private iniciarSubstitutos() {
    this.service.listarSubstitutos().subscribe((substitutos) => {
      this.listaSubstitutos = substitutos;

    });

  }

  async formInit() {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.dialog.closeAll();
      }
    });

    this.formDadosDocumento = this.formBuilder.group({
      descricao: new FormControl(null, [
        Validators.required,
        Validators.maxLength(640),
        this.validarLinhasDescricao,
      ]),
      referencia: new FormControl(null, [
        Validators.required,
        Validators.maxLength(100),
      ]),
      tipoMoeda: new FormControl(null, [Validators.required]),
      valor: new FormControl(null, [
        Validators.required,
        Validators.maxLength(15),
      ]),
      fornecedor: new FormControl(null, [Validators.required]),
      tipoPagamento: new FormControl(null, [Validators.required]),
      parcelas: new FormControl(null, [Validators.pattern('[0-9]*')]),
      vencimentoDias: new FormControl(null, [Validators.pattern('[0-9]*')]),
      prazoEntrega:  new FormControl(null, [
        Validators.maxLength(20),
      ]),
      destaque: new FormControl(),
      referenciaSubstituto: new FormControl(),
    });

    this.formValidationHelper = new FormValidationHelper({
      descricao: 'Descrição da necessidade',
      referencia: 'Referência',
      tipoMoeda: 'Moeda',
      valor: 'Valor',
      fornecedor: 'Fornecedor',
      tipoPagamento: 'Tipo de pagamento',
      parcelas: 'Qtde. parcelas',
      vencimentoDias: 'Vencimento para (dias)',
      prazoEntrega: 'Prazo de entrega (dias)',
      destaque: 'Este documento é de ALTA PRIORIDADE (Prazo, Relevância, etc)'
    });


    this.formDadosDocumento.valueChanges.subscribe(form => {
        this.documentoModelModelChange.emit(...Object.assign(this.documentoModel,form));
        this.emitirEventoDeErrosFormStatusPagamento();

    });


    this.emitirEventoDeErrosFormStatusPagamento();
  }

  private emitirEventoDeErrosFormStatusPagamento(){
    setTimeout(() => {
      this.errorsChange.emit(this.formValidationHelper.getFormValidationErrorsMessages(this.formDadosDocumento.controls));
    });
  }

    validarFormulario():boolean {
    this.tipoPagamentoValidator();
    this.validarColunasDescricao();

    const errorList: Array<string> =
      this.formValidationHelper.getFormValidationErrorsMessages(
        this.formDadosDocumento.controls
      );

      let valor = this.formDadosDocumento.value.valor != null ?Number(this.formDadosDocumento.value.valor.replace(',', '.')):0;
      if(valor == 0 || valor == undefined || Number.isNaN(valor))
      errorList.push("O Valor deve ser maior que 0,00");

    
      if(this.formDadosDocumento.value.referenciaSubstituto != null){
        if(this.listaSubstitutos.find(x => x.id == this.formDadosDocumento.value.referenciaSubstituto).flgEmAprovacaoCiencia){
          errorList.push("Está pendente aprovação de ciência do Status de Pagamento referenciado. Somente após a ciência de cancelamento o Status de Pagamento em substituição poderá ser enviado.");
        }
      }


    if (errorList.length > 0) {
      this.errorsChange.emit(errorList);
      return false;
    }

    return true;
  }


  private tipoPagamentoValidator() {
    this.formDadosDocumento.controls['parcelas'].clearValidators();

    if (this.habilitarCampoParcela()) {
      this.formDadosDocumento.controls['parcelas'].setValidators([
        Validators.required,
        Validators.pattern('[0-9]*'),
      ]);
    } else {
      this.formDadosDocumento.controls['parcelas'].setValidators([
        Validators.pattern('[0-9]*'),
      ]);
    }
    this.formDadosDocumento.controls['parcelas'].updateValueAndValidity();
  }

  tipoPagamentouChange(){
      this.tipoPagamentoValidator()
  }

  private validarColunasDescricao() {
    let textoDescricao = this.formDadosDocumento.controls['descricao'].value as string;

    if (!textoDescricao) {
      return;
    }

    const linhas = textoDescricao.split('\n');

    textoDescricao = ''

    for (let indexLinha = 0; indexLinha < linhas.length; indexLinha++) {
      const linha = linhas[indexLinha];

      const estaNaUltimaLinha = indexLinha + 1 === linhas.length;

      textoDescricao += this.realizarQuebrasDeLinhaCasoNecessario(linha, estaNaUltimaLinha);
    }

    this.formDadosDocumento.controls['descricao'].setValue(textoDescricao);
  }

  private realizarQuebrasDeLinhaCasoNecessario(linha: string, estaNaUltimaLinha: boolean) {
    let textoFinalDaLinha = '';

    if (linha.length > this.descricaoMaximoColunas) {
      const quantidadeDeQuebras = Math.ceil(linha.length / this.descricaoMaximoColunas);
      let inicio = 0;

      for (let indexQuabraDeLinha = 0; indexQuabraDeLinha < quantidadeDeQuebras; indexQuabraDeLinha++) {
        const linhaAteMaximo = linha.substring(inicio, inicio + this.descricaoMaximoColunas);

        if (linhaAteMaximo.length >= this.descricaoMaximoColunas) {
          const pontoDeQuebra = linhaAteMaximo.lastIndexOf(' ');

          textoFinalDaLinha += `${linhaAteMaximo.substring(0, pontoDeQuebra)}\n${linhaAteMaximo.substring(pontoDeQuebra, linhaAteMaximo.length)}`;
        } else {
          textoFinalDaLinha += linhaAteMaximo;
        }

        inicio += this.descricaoMaximoColunas;
      }
    } else {
      textoFinalDaLinha += estaNaUltimaLinha ? linha : linha + '\n';
    }

    return textoFinalDaLinha;
  }

  private validarLinhasDescricao(input: FormControl) {
    const validacao = CustomFormValidationsHelper.MaxRows(
        AppConfig.settings.statusPagamento.maxRowsDescricao,
        input.value
      );

      if (!validacao) {
        return validacao;
      }

      return { maxRows: true };
  }

  private montarModelComValoresDoFormulario(): void {
    const tipoPagto = this.formDadosDocumento.value.tipoPagamento;
    this.documentoModel.ativo = true;
    this.documentoModel.status = StatusDocumento.EmConstrucao;
    this.documentoModel.referenciaSubstituto = this.formDadosDocumento.value.referenciaSubstituto;
    this.documentoModel.descricao = this.formDadosDocumento.value.descricao;
    this.documentoModel.referencia = this.formDadosDocumento.value.referencia;
    if(this.formDadosDocumento.value.valor != undefined){
      this.documentoModel.valor = Number(
        this.formDadosDocumento.value.valor.replace(',', '.')
        );
      }
    this.documentoModel.tipoPagamento = tipoPagto;

    if (tipoPagto == TiposPagamento.Parcelado) {
      this.documentoModel.quantidadeParcelas =
        this.formDadosDocumento.value.parcelas;
    } else {
      this.documentoModel.quantidadeParcelas = null;
    }

    this.documentoModel.moedaSimbolo = this.formDadosDocumento.value.tipoMoeda;
    this.documentoModel.fornecedor =   this.formDadosDocumento.value.fornecedor;

    if (tipoPagto == TiposPagamento.Unico) {
      this.documentoModel.vencimentoPara =
        this.formDadosDocumento.value.vencimentoDias;
    } else {
      this.documentoModel.vencimentoPara = null;
    }

    this.documentoModel.prazoEntrega = this.formDadosDocumento.value.prazoEntrega;
    this.documentoModel.destaque = this.formDadosDocumento.value.destaque;

    if(this.editarDocumento !== undefined){
        this.documentoModel.id = this.editarDocumento.id;
        this.documentoModel.numero = this.editarDocumento.numero;
        this.documentoModel.autorId = this.editarDocumento.autorId;
    }

  }

  habilitarCampoParcela(): boolean {
    if (this.formDadosDocumento.value.tipoPagamento === null) {
      return false;
    }
    return this.formDadosDocumento.value.tipoPagamento === TiposPagamento.Parcelado;
  }

  habilitarCampoVencimentoPara(): boolean {
    if (this.formDadosDocumento.value.tipoPagamento === null) {
      return false;
    }
    return this.formDadosDocumento.value.tipoPagamento === TiposPagamento.Unico;
  }

  BuildFormulario(): void {
    this.montarModelComValoresDoFormulario();
    this.documentoModelModelChange.emit(...Object.assign(this.documentoModel,[]));
  }


  apenasNumeroEVirgula(event): boolean {
    const charCode = event.which ? event.which : event.keyCode;
    if (charCode > 31 && charCode !== 44 && (charCode < 48 || charCode > 57)) {
      return false;
    }
    return true;
  }

  onBlurValor(valor: string) {
    if (!valor) {
      valor = '0,00';
    }

    if (!valor.includes(',')) {
      valor = `${valor},00`;
    }else{
      const casasDecimais = valor.split(",")
      if(casasDecimais[1].length == 1){
        valor = `${valor}0`;
      }
    }

    this.formDadosDocumento.controls['valor'].setValue(valor);
  }

  async EditarDocumento(){
    const dfi_idt = this.activedRoute.snapshot.paramMap.get('dfi_idt');
    if (dfi_idt !== null && dfi_idt !== undefined && dfi_idt !== '' && dfi_idt != '0' && !isNaN(Number(dfi_idt.toString()))) {
        await this.buscaStatusPagamento(dfi_idt);
    }

    if(this.editarDocumento !== undefined ){
      this.PreencherEdicaoForm()
    }

    if(this.editarDocumento.referenciaSubstituto != null)
      {
        this.ObterDocumentoSubstitutoProprio(this.editarDocumento.referenciaSubstituto);

      }

  }

  ObterDocumentoSubstitutoProprio(dfi_idt){

    this.service.obterStatusPagamentoPorId(dfi_idt)
    .toPromise()
    .then((statusPagamento) => {
      if(statusPagamento !== undefined && statusPagamento != null){
        this.listaSubstitutos.push({
          id : statusPagamento.id,
          descricao: statusPagamento.descricao,
          valor: statusPagamento.numero + " - " + statusPagamento.descricao ,
          flgEmAprovacaoCiencia:false
         })

         this.listaSubstitutos = [...this.listaSubstitutos];

          this.formDadosDocumento.get('referenciaSubstituto').setValue(this.editarDocumento.referenciaSubstituto);
      }
    })
  }

  private PreencherEdicaoForm()
  {

    this.formDadosDocumento.get('descricao').setValue(this.editarDocumento.descricao);
    this.formDadosDocumento.get('referencia').setValue(this.editarDocumento.referencia);
    this.formDadosDocumento.controls['tipoMoeda'].setValue(this.editarDocumento.moedaSimbolo);
    this.formDadosDocumento.get('valor').setValue(this.editarDocumento.valor);
    let valorAux= this.formDadosDocumento.value.valor.toString();
    valorAux = valorAux.replace(".",",");
    this.formDadosDocumento.value.valor=valorAux;
    this.editarDocumento.valor = valorAux;
    this.onBlurValor(this.editarDocumento.valor.toString());
    this.formDadosDocumento.get('fornecedor').setValue(this.editarDocumento.fornecedor);
    this.formDadosDocumento.get('prazoEntrega').setValue(this.editarDocumento.prazoEntrega);
    this.formDadosDocumento.get('destaque').setValue(this.editarDocumento.destaque);
    this.formDadosDocumento.get('tipoPagamento').setValue(this.editarDocumento.tipoPagamento);




    if(this.editarDocumento.vencimentoPara != null && this.editarDocumento.vencimentoPara != 0)
      this.formDadosDocumento.get('vencimentoDias').setValue(this.editarDocumento.vencimentoPara);

    if(this.editarDocumento.quantidadeParcelas != null && this.editarDocumento.quantidadeParcelas != 0)
      this.formDadosDocumento.get('parcelas').setValue(this.editarDocumento.quantidadeParcelas);
  }


  async buscaStatusPagamento(dfi_idt){
      await this.service.obterStatusPagamentoPorId(dfi_idt)
      .toPromise()
      .then((statusPagamento) => {
        if(statusPagamento !== undefined && statusPagamento != null){
          this.editarDocumento = statusPagamento
          this.validaStatus();
        }
      })
    }


  validaStatus(){
    if(StatusDocumento[this.editarDocumento.status.toString()] != StatusDocumento.EmConstrucao){
     this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Não é possível editar o documento, pois ele não se encontra no status "Em construção".')
      .then(() => {
        this.router.navigate(['fi1548/consultar']);
      })
      .catch((result)=> {console.error(result)});
    }
  }

}
