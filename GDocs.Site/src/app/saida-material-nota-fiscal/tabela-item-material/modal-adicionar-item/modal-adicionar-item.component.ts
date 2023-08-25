import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase, ExibicaoDeAlertaService } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { ItemSaidaMaterialNF } from 'src/app/saida-material-nota-fiscal/shared/models/ItemSaidaMaterialNF';

@Component({
  selector: 'app-modal-adicionar-item',
  templateUrl: './modal-adicionar-item.component.html'
})
export class ModalAdicionarItemComponent extends ComponentFormBase implements OnInit {

  formAdicionarMaterial: FormGroup;
  formValidadorAuxiliar: FormValidationHelper;
  itemSaidaMaterial: ItemSaidaMaterialNF
  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    public dialogRef: MatDialogRef<any>,

    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {
    this.formAdicionarMaterial = this.formBuilder.group({
      quantidade:[null,  [Validators.required, Validators.pattern('^$|[0-9]*')]],
      unidade:[null, [Validators.required, Validators.maxLength(5),Validators.pattern('[a-zA-Z ]*')]],
      valorUnitario:[null,[Validators.maxLength(10),Validators.required]],
      codigo:[null,[Validators.maxLength(10),Validators.required]],
      tagService:[null,[Validators.maxLength(10)]],
      descricao:[null, [Validators.required, Validators.maxLength(150)]],
      patrimonio:[null,[Validators.maxLength(10)]],
    });

    this.formValidadorAuxiliar = new FormValidationHelper({
      descricao: 'Descrição',
      quantidade:"Quantidade",
      unidade:"Unidade",
      codigo:"Código",
      valorUnitario:"Valor Unitário",
      tagService:"Tag Service",
      patrimonio:"Patrimônio"
    });

    if(this.data.item !== undefined )
      this.PreencherFormularioEdicao()

  }
  apenasNumeroOuVirgula(event): boolean {
    const codigoEvento = event.which ? event.which : event.keyCode;
    if (codigoEvento > 31 && codigoEvento !== 44 && (codigoEvento < 48 || codigoEvento > 57)) {
      return false;
    }
    return true;
  }

 private PreencherFormularioEdicao()
  {
    this.formAdicionarMaterial.get('descricao').setValue(this.data.item.descricao);
    this.formAdicionarMaterial.get('quantidade').setValue(this.data.item.quantidade);
    this.formAdicionarMaterial.get('unidade').setValue(this.data.item.unidade);
    this.formAdicionarMaterial.get('patrimonio').setValue(this.data.item.patrimonio);
    this.formAdicionarMaterial.get('valorUnitario').setValue(this.data.item.valorUnitario);
    this.formAdicionarMaterial.get('codigo').setValue(this.data.item.codigo);
    this.formAdicionarMaterial.get('tagService').setValue(this.data.item.tagService);
    this.onBlurValor(this.data.item.valorUnitario);
  }


  confirmarSalvarModal() {
    if (!this.validarFormAdicionarMaterial()) {
      return;
    }

      const NovoItemSolicitacao = this.itemSaidaMaterial = new ItemSaidaMaterialNF()
      NovoItemSolicitacao.quantidade = this.formAdicionarMaterial.controls['quantidade'].value;
      NovoItemSolicitacao.unidade = this.formAdicionarMaterial.controls['unidade'].value;
      NovoItemSolicitacao.descricao = this.formAdicionarMaterial.controls['descricao'].value;
      NovoItemSolicitacao.codigo = this.formAdicionarMaterial.controls['codigo'].value;
      NovoItemSolicitacao.valorUnitario = Number(this.formAdicionarMaterial.controls['valorUnitario'].value.replace(',', '.'));

      NovoItemSolicitacao.tagService = this.formAdicionarMaterial.controls['tagService'].value;
      NovoItemSolicitacao.patrimonio = this.formAdicionarMaterial.controls['patrimonio'].value;

     this.dialogRef.close(NovoItemSolicitacao);
  }


  onBlurValor(valor: string) {
    if (!valor) {
      valor = '0,00';
    }
    if (valor.toString().includes('.')) {
      valor =  valor.toString().replace('.',',')
    }
    if (!valor.toString().includes(',')) {
      valor = `${valor},00`;
    }
    this.formAdicionarMaterial.controls['valorUnitario'].setValue(valor);
  }

  fecharModal() {
    this.dialogRef.close();
  }


  private validarFormAdicionarMaterial(): boolean {
    this.formAdicionarMaterial.get('descricao').setValue(this.formAdicionarMaterial.value.descricao === null ? null : this.formAdicionarMaterial.value.descricao);
    this.formAdicionarMaterial.get('quantidade').setValue(this.formAdicionarMaterial.value.quantidade === null ? null : this.formAdicionarMaterial.value.quantidade);
    this.formAdicionarMaterial.get('unidade').setValue(this.formAdicionarMaterial.value.unidade === null ? null : this.formAdicionarMaterial.value.unidade);
    this.formAdicionarMaterial.get('codigo').setValue(this.formAdicionarMaterial.value.codigo === null ? null : this.formAdicionarMaterial.value.codigo);
    this.formAdicionarMaterial.get('valorUnitario').setValue(this.formAdicionarMaterial.value.valorUnitario === null ? null : this.formAdicionarMaterial.value.valorUnitario);
    this.formAdicionarMaterial.get('tagService').setValue(this.formAdicionarMaterial.value.tagService === null ? null : this.formAdicionarMaterial.value.tagService);
    this.formAdicionarMaterial.get('patrimonio').setValue(this.formAdicionarMaterial.value.patrimonio === null ? null : this.formAdicionarMaterial.value.patrimonio);

    const valorUnitario = Number(this.formAdicionarMaterial.controls['valorUnitario'].value != null?this.formAdicionarMaterial.controls['valorUnitario'].value.replace(',', '.'):0);
    const listaDeErros: Array<string> = this.formValidadorAuxiliar.getFormValidationErrorsMessages(this.formAdicionarMaterial.controls);

    if (valorUnitario == null || valorUnitario == undefined || Number.isNaN(valorUnitario) ) {
      listaDeErros.push('o campo valor unitário esta no formato errado.');
    }

    if (listaDeErros.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', listaDeErros.join('<br />'));
      return false;
    }

    return true;
  }
}
