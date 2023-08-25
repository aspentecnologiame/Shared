import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from '../../core/component-form-base';
import { ExibicaoDeAlertaService } from '../../../app/core/services/exibicao-de-alerta/exibicao-de-alerta.service';
import { FormValidationHelper } from '../../helpers';
import { ItemSaidaMateialModel } from 'src/app/FI347/shared/models/item-saida-material.model';

@Component({
  selector: 'app-material-saida-modal',
  templateUrl: './material-saida-modal.component.html'
})
export class MaterialSaidaModalComponent extends ComponentFormBase implements OnInit {

  form: FormGroup;
  formValidationHelper: FormValidationHelper;
  itemSaidaMaterial: ItemSaidaMateialModel
  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    public dialogRef: MatDialogRef<any>,

    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      descricao:[null, [Validators.required, Validators.maxLength(150)]],
      unidade:[null, [Validators.required, Validators.maxLength(5),Validators.pattern('[a-zA-Z ]*')]],
      patrimonio:[null,Validators.maxLength(10)],
      quantidade:[null,  [Validators.required, Validators.pattern('^$|[0-9]*')]]

    });

    this.formValidationHelper = new FormValidationHelper({
      descricao: 'Descrição',
      quantidade:"Quantidade",
      unidade:"Unidade",
      patrimonio:"Patrimônio"

    });

    if(this.data.editarItem !== undefined )
      this.PreencherFormularioEdicao()

  }


 private PreencherFormularioEdicao()
  {
    this.form.get('descricao').setValue(this.data.editarItem.descricao);
    this.form.get('quantidade').setValue(this.data.editarItem.quantidade);
    this.form.get('unidade').setValue(this.data.editarItem.unidade);
    this.form.get('patrimonio').setValue(this.data.editarItem.patrimonio);
  }



  confirmarModal() {
    if (!this.validarFormulario()) {
      return;
    }


    const SaidaDeMaterial = this.itemSaidaMaterial = new ItemSaidaMateialModel(
      this.form.controls['quantidade'].value,
      this.form.controls['unidade'].value,
      this.form.controls['descricao'].value,
      this.form.controls['patrimonio'].value,
      this.RetornaId()
       );

     this.dialogRef.close(SaidaDeMaterial);
  }





  cancelarModal() {
    this.dialogRef.close();
  }

  private RetornaId():number{
    return (this.data.editarItem !== undefined ? this.data.editarItem.idSolicitacaoSaidaMaterialItem:0  )
  }

  private validarFormulario(): boolean {
    this.form.get('descricao').setValue(this.form.value.descricao === null ? null : this.form.value.descricao);
    this.form.get('quantidade').setValue(this.form.value.quantidade === null ? null : this.form.value.quantidade);
    this.form.get('unidade').setValue(this.form.value.unidade === null ? null : this.form.value.unidade);


    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.form.controls);

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }
}
