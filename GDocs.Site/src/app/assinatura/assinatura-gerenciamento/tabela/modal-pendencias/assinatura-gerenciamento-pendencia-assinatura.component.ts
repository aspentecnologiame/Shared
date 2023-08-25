import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AssinaturaModel } from 'src/app/assinatura/models';

@Component({
  selector: 'app-assinatura-gerenciamento-pendencia-assinatura',
  templateUrl: './assinatura-gerenciamento-pendencia-assinatura.component.html',
  styleUrls: ['./assinatura-gerenciamento-pendencia-assinatura.component.scss']
})
export class AssinaturaGerenciamentoPendenciaAssinaturaComponent {

  constructor(
    public dialogRef: MatDialogRef<AssinaturaGerenciamentoPendenciaAssinaturaComponent>,
    @Inject(MAT_DIALOG_DATA) public assinaturaProcesso: AssinaturaModel
  ) { }

  fecharModal(){
    this.dialogRef.close();
  }
}
