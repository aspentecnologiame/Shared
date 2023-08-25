import { HistoricoTrocaAnexoSaidaModel } from './../shared/models/Historico-troca-anexo-saida-model';
import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { SaidaMaterialNotaFiscal } from '../shared/models/SaidaMaterialNotaFiscal';
import { StatusAcaoMaterialNotaFiscal } from '../shared/enums/StatusAcaoMaterialNotaFiscal';
import { HistoricoProrrogacaoNotaFiscalModel } from '../shared/models/HistoricoProrrogacaoNotaFiscalModel';
import { ItemSaidaMaterialNF } from '../shared/models/ItemSaidaMaterialNF';
import { SaidaMaterialNotafiscalAcaoModel } from '../shared/models/SaidaMaterialNotafiscalAcaoModel';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';

@Component({
  selector: 'app-historico-nota-fiscal',
  templateUrl: './historico-nota-fiscal.component.html',
  styleUrls: ['./historico-nota-fiscal.component.scss']
})
export class HistoricoNotaFiscalComponent extends ComponentFormBase  implements OnInit {
  public get dialogRef(): MatDialogRef<any> {
    return this._dialogRef;
  }
  public set dialogRef(value: MatDialogRef<any>) {
    this._dialogRef = value;
  }

  form: FormGroup;
  formValidationHelper: FormValidationHelper;
  SaidaMaterial: SaidaMaterialNotaFiscal;
  acao:StatusAcaoMaterialNotaFiscal;
  acaoTabelaHistorico = StatusAcaoMaterialNotaFiscal.BaixaMaterialNFSemRetorno;
  SaidaAcao:SaidaMaterialNotafiscalAcaoModel[] =[];
  DataSourceItemMaterial: ItemSaidaMaterialNF[] = [];
  DataSourceHistoricoProrogacao:HistoricoProrrogacaoNotaFiscalModel[] =[];
  DataSourceHistoricoBaixaSemRetorno:HistoricoProrrogacaoNotaFiscalModel[] =[];
  DataSourceHistoricoTrocaNfSaida:HistoricoTrocaAnexoSaidaModel[]=[];
  DataSourceAcaoSaida:SaidaMaterialNotafiscalAcaoModel;
  DataSourceAcaoRetorno:SaidaMaterialNotafiscalAcaoModel;


  ItemMarcadoid:number;

  constructor(
    private _dialogRef: MatDialogRef<any>,
    private eventMiddleService: EventMiddleService,

    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit() {
    this.acao = StatusAcaoMaterialNotaFiscal.HistoricoMaterial;
    this.SaidaMaterial =  this.data.historico.saidaMaterialNotaFiscal;
    this.DataSourceItemMaterial = this.data.historico.saidaMaterialNotaFiscal.itemMaterialNf
    this.SaidaAcao = this.data.historico.saidaMaterialNotaFiscalAcoes;
    this.DataSourceHistoricoProrogacao = this.data.historico.historicoProrrogacoes;
    this.DataSourceAcaoSaida = this.SaidaAcao.find(x => x.idSaidaMaterialNotaFiscalTipoAcao == 1)
    this.DataSourceHistoricoTrocaNfSaida = this.data.historico.historicoTrocaAnexoSaida;
    this.FechaModalHistoricoNf();
  }

  FechaModalHistoricoNf(){
    this.eventMiddleService.getEvent()
    .subscribe(evento => {
      if(evento != null && evento != undefined){
        this.dialogRef.close();
      }
    });
  }

  ItemMarcado(itemId:number){
    this.ItemMarcadoid = itemId;
    this.DataSourceAcaoRetorno = this.SaidaAcao.find(x => x.idSaidaMaterialNotaFiscalAcaoItem == itemId &&
                                                          x.idSaidaMaterialNotaFiscalTipoAcao == StatusAcaoMaterialNotaFiscal.Retorno.valueOf());
    this.DataSourceHistoricoBaixaSemRetorno = this.data.historico.historicoBaixaSemRetorno.filter(x => x.saidaMaterialNotaFiscalItemId == itemId);
  }

  cancelarModal() {
    this.dialogRef.close();
  }

  headerTitulo(numero):string{
    if(numero != undefined || numero != null){
      return `- NF ${numero}`
    }
    return ""
  }

}
