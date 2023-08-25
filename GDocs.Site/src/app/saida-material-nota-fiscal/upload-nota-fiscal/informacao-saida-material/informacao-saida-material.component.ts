import { SaidaMaterialNotaFiscal } from './../../shared/models/SaidaMaterialNotaFiscal';
import { Component, OnInit, Input } from '@angular/core';
@Component({
  selector: 'app-informacao-saida-material',
  templateUrl: './informacao-saida-material.component.html',
  styleUrls: ['informacao-saida-material.component.scss']
})
export class InformacaoSaidaMaterialComponent implements OnInit {

  @Input() dataSource:SaidaMaterialNotaFiscal

  ngOnInit() {
    this.VerificarDataSource();
  }

  VerificarDataSource():boolean{
    return this.dataSource != null && this.dataSource != undefined
  }

  MostrarFornecedor():boolean{
    return this.dataSource.fornecedor != null && this.dataSource.fornecedor != undefined;
  }


}

