import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ExibicaoDeAlertaService } from '../../../app/core/services/exibicao-de-alerta/exibicao-de-alerta.service';
import { ComponentFormBase } from '../../core/component-form-base';
import { FormValidationHelper } from '../../helpers';

export interface DialogData {
  titulo: string;
  confirmar: string;
  nomeCampo: string;
  maxLength: number;
}

@Component({
  selector: 'app-justificativa-modal',
  templateUrl: './justificativa-modal.component.html',
  styleUrls: ['./justificativa-modal.component.scss']
})
export class JustificativaModalComponent extends ComponentFormBase implements OnInit {
  form: FormGroup;
  formValidationHelper: FormValidationHelper;
  DescricaoCampo: string = 'Descrição';

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    public dialogRef: MatDialogRef<any>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {
    super();
  }

  ngOnInit() {
      if(this.data.nomeCampo != null  && this.data.nomeCampo != undefined)
      this.DescricaoCampo = this.data.nomeCampo

    this.form = this.formBuilder.group({
      nomeCampo: [null, Validators.required]
    });

    this.formValidationHelper = new FormValidationHelper({
      nomeCampo: this.DescricaoCampo
    });

    if(this.data.maxLength != null  && this.data.maxLength != undefined  &&  this.data.maxLength != 0)
    this.form.get('nomeCampo').setValidators([Validators.required, Validators.maxLength(this.data.maxLength)]);
  }

  confirmarModal() {
    if (!this.validarFormulario()) {
      return;
    }

    this.dialogRef.close({ justificativa: this.form.controls['nomeCampo'].value });
  }

  cancelarModal() {
    this.dialogRef.close();
  }

  private validarFormulario(): boolean {
    this.form.get('nomeCampo').setValue(this.form.value.nomeCampo === null ? null : this.form.value.nomeCampo.trim());

    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.form.controls);

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }
}
