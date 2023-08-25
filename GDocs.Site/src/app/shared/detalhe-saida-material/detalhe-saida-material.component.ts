import { Component, Input, } from '@angular/core';
import { RegistroSaidaMaterial } from 'src/app/FI347/shared/models/registro-saida-material';

@Component({
  selector: 'app-detalhe-saida-material',
  templateUrl: './detalhe-saida-material.component.html',
})
export class DetalheSaidaMaterialComponent  {

  msgNaoEncontrado:string;

  @Input() dataSource: RegistroSaidaMaterial;

  @Input()
  set MsgNaoEncontrado(msg:string) {
      this.msgNaoEncontrado = msg;
  }


  DadosEncontrado():boolean{
      return this.dataSource != undefined
  }
}
