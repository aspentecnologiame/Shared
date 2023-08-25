import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { AppConfig } from '../../../app.config';
import { FormValidationHelper } from 'src/app/helpers';
import { AssinaturaService } from '../../assinatura.service';
import { AssinaturaCategoriaModel, AssinaturaInformacoesModel } from '../../models';
@Component({
  selector: 'app-assinatura-informacoes',
  templateUrl: './assinatura-informacoes.component.html',
  styleUrls: ['./assinatura-informacoes.component.scss']
})
export class AssinaturaInformacoesComponent implements OnInit {
  @Input() assinaturaInformacoesModel: AssinaturaInformacoesModel = new AssinaturaInformacoesModel();
  @Output() assinaturaInformacoesModelChange = new EventEmitter<AssinaturaInformacoesModel>();

  @Input() errors: string[];
  @Output() errorsChange = new EventEmitter<string[]>();

  semGerarNumero = false;
  categorias: AssinaturaCategoriaModel[];

  formAssinaturaInformacoes: FormGroup;
  formValidationHelper: FormValidationHelper;

  numeracaoAutomatica: boolean;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly assinaturaService: AssinaturaService
  ) {
    this.formValidationHelper = new FormValidationHelper({
      titulo: 'Título do fluxo',
      categoriaId: 'Categoria',
      descricao: 'Descrição',
      nomeDocumento: 'Nome do documento',
      numeroDocumento: 'Número do documento',
      destaque: 'Destacar'
    });
  }

  ngOnInit() {
    this.formInit();
  }

  formInit() {
    this.formAssinaturaInformacoes = this.formBuilder.group({
      titulo: new FormControl(null, [Validators.required, Validators.maxLength(255)]),
      categoriaId: new FormControl(null, [Validators.required]),
      descricao: new FormControl(null, [Validators.required]),
      nomeDocumento: new FormControl(null, [Validators.required, Validators.maxLength(255)]),
      numeroDocumento: new FormControl(null, [Validators.required, Validators.pattern('^$|[0-9]*')]),
      destaque: new FormControl(false),
      gerarNumeroDocumento: new FormControl(false),
      numeroDocumentoAutomatico: new FormControl(false),
      chaveNumeroDocumentoAutomatico: new FormControl(''),
      certificadoDigital: new FormControl(false),
      assinaturaDocumento: new FormControl(false)
    });

    this.carregarCategorias();

    this.formAssinaturaInformacoes.valueChanges.subscribe(form => {
      if(form.numeroDocumento === ''){
        form.numeroDocumento = null;
      }

      this.assinaturaInformacoesModel = {
        ...Object.assign(
        this.assinaturaInformacoesModel,
        form,
        { numeroDocumento: form.numeroDocumento == null ? form.numeroDocumento : parseInt(form.numeroDocumento) })
      };
      this.assinaturaInformacoesModelChange.emit(this.assinaturaInformacoesModel);
      this.emitirEventoDeErrosDoFormularioInformacoes();
    });

    this.emitirEventoDeErrosDoFormularioInformacoes();
  }

  private carregarCategorias(): void {
    this.assinaturaService.listarAssinaturaCategoriaExclude()
      .subscribe(
        resposta => {
          this.categorias = resposta;
        },
        error => {
          console.error('Erro no método carregar categorias.', error);
        }
      );
  }

  private emitirEventoDeErrosDoFormularioInformacoes(){
    setTimeout(() => {
      this.errorsChange.emit(this.formValidationHelper.getFormValidationErrorsMessages(this.formAssinaturaInformacoes.controls));
    });
  }

  gerarNumeroDocumento(checked: boolean) {
    this.semGerarNumero = checked;
    this.formAssinaturaInformacoes.controls['gerarNumeroDocumento'].setValue(this.semGerarNumero);

    if(this.semGerarNumero) {
      this.formAssinaturaInformacoes.controls['numeroDocumento'].setValue(null);
      this.definirValidacoesCampoNumerodocumento(false);
    } else {
      this.definirValidacoesCampoNumerodocumento(true);
    }
  }

  checarCategoriaSelecionada(idCategoria: any) {
    const categoria = AppConfig.settings.categorias.find(w => w.codigo === idCategoria);
    if (categoria !== undefined && categoria.numeracaoAutomatica.habilitado) {
      this.formAssinaturaInformacoes.controls['numeroDocumento'].setValue(null);
      this.definirValidacoesCampoNumerodocumento(false);
      this.semGerarNumero = false;
      this.formAssinaturaInformacoes.controls['gerarNumeroDocumento'].setValue(this.semGerarNumero);
      this.formAssinaturaInformacoes.controls['chaveNumeroDocumentoAutomatico'].setValue(categoria.numeracaoAutomatica.chaveSequence);
      this.formAssinaturaInformacoes.controls['certificadoDigital'].setValue(categoria.certificadoDigital.habilitado);
      this.formAssinaturaInformacoes.controls['assinaturaDocumento'].setValue(categoria.assinaturaDocumento.habilitado);
      this.numeracaoAutomatica = true;
    } else {
      this.formAssinaturaInformacoes.controls['chaveNumeroDocumentoAutomatico'].setValue('');
      this.formAssinaturaInformacoes.controls['certificadoDigital'].setValue(false);
      this.formAssinaturaInformacoes.controls['assinaturaDocumento'].setValue(false);
      this.numeracaoAutomatica = false;
      this.definirValidacoesCampoNumerodocumento(true);
    }
    this.formAssinaturaInformacoes.controls['numeroDocumentoAutomatico'].setValue(this.numeracaoAutomatica);
  }

  definirValidacoesCampoNumerodocumento(ativaValidacoes: boolean) {
    this.formAssinaturaInformacoes.controls['numeroDocumento'].clearValidators();
    if (ativaValidacoes) {
      this.formAssinaturaInformacoes.controls['numeroDocumento'].setValidators([Validators.required, Validators.maxLength(255)]);
    }
    this.formAssinaturaInformacoes.controls['numeroDocumento'].updateValueAndValidity();
  }

  montarPlaceHolderNumeracaoAutomatica(): string {
    if (this.numeracaoAutomatica !== undefined && this.numeracaoAutomatica) {
      return "* Numeração Automática *";
    }
    if (this.semGerarNumero) {
      return "";
    }
    return "Número do documento";
  }
}
