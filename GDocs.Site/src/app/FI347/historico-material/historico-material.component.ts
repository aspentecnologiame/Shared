import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from 'src/app/core';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';
import { FormValidationHelper } from 'src/app/helpers';
import { SaidaMaterialModel } from '../shared';
import { StatusAcaoMaterial } from '../shared/enums/status-acao-materia';
import { HistorioProrrogacaoModel } from '../shared/models/historico-prorrogacao.model';
import { ItemSaidaMateialModel } from '../shared/models/item-saida-material.model';
import { RegistroSaidaMaterial } from '../shared/models/registro-saida-material';

@Component({
  selector: 'app-historico-material',
  templateUrl: './historico-material.component.html',
  styleUrls: ['./historico-material.component.scss']
})
export class HistoricoMaterialComponent extends ComponentFormBase  implements OnInit {
  public get dialogRef(): MatDialogRef<any> {
    return this._dialogRef;
  }
  public set dialogRef(value: MatDialogRef<any>) {
    this._dialogRef = value;
  }

  form: FormGroup;
  formValidationHelper: FormValidationHelper;
  SaidaMaterial: SaidaMaterialModel;
  acao:StatusAcaoMaterial;
  acaoTabelaHistorico = StatusAcaoMaterial.BaixaMaterialSemRetorno;
  SaidaAcao:RegistroSaidaMaterial[] =[];
  DataSourceItemMaterial: ItemSaidaMateialModel[] = [];
  DataSourceHistoricoProrogacao:HistorioProrrogacaoModel[] =[];
  DataSourceHistoricoBaixaSemRetorno:HistorioProrrogacaoModel[] =[];
  DataSourceAcaoSaida:RegistroSaidaMaterial;
  DataSourceAcaoRetorno:RegistroSaidaMaterial;


  ItemMarcadoid:number;

  constructor(
    private _dialogRef: MatDialogRef<any>,
    private eventMiddleService: EventMiddleService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {

    this.acao = StatusAcaoMaterial.HistoricoMaterial;
    this.SaidaMaterial =  this.data.historico.solicitacaoMaterial;
    this.DataSourceItemMaterial = this.data.historico.solicitacaoMaterial.itemMaterial;
    this.SaidaAcao = this.data.historico.solicitacaoMaterialAcao;
    this.DataSourceHistoricoProrogacao = this.data.historico.historicoProrogacao;
    this.DataSourceAcaoSaida = this.SaidaAcao.find(x => x.idSaidaMaterialTipoAcao == 1);
    this.ExpirouSessaoFecharModalHistorico();
  }



  ItemMarcado(itemId:number){
    this.ItemMarcadoid = itemId;
    this.DataSourceAcaoRetorno = this.SaidaAcao.find(x => x.solicitacaoMaterialAcaoItemId == itemId &&
                                                          x.idSaidaMaterialTipoAcao == StatusAcaoMaterial.RegistroRetorno.valueOf());
    this.DataSourceHistoricoBaixaSemRetorno = this.data.historico.historicoBaixaSemRetorno.filter(x => x.solicitacaoMaterialItemId == itemId);
  }

  cancelarModal() {
    this.dialogRef.close();
  }

  ExpirouSessaoFecharModalHistorico(){
    this.eventMiddleService.getEvent()
    .subscribe(e => {
      if(e != null && e != undefined){
        this._dialogRef.close();
      }
    });
  }

}
