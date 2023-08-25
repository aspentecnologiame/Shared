import { CienciaUsuarioAprovacaoModel } from './../../../../saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from 'src/app/core';
import { StatusCiencia } from 'src/app/FI347/shared/enums/status-ciencia';
import { HistorioProrrogacaoModel } from 'src/app/FI347/shared/models/historico-prorrogacao.model';
import { FormValidationHelper } from 'src/app/helpers';
import { StatusAcaoMaterialNotaFiscal } from 'src/app/saida-material-nota-fiscal/shared/enums/StatusAcaoMaterialNotaFiscal';
import { ItemSaidaMaterialNF } from 'src/app/saida-material-nota-fiscal/shared/models/ItemSaidaMaterialNF';
import { SaidaMaterialNotaFiscalCienciaModel } from 'src/app/saida-material-nota-fiscal/shared/models/saida-material-nota-fiscal-ciencia-model';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';

@Component({
  selector: 'app-assinatura-ciencia-material-nota-fiscal-acao',
  templateUrl: './assinatura-ciencia-material-nota-fiscal-acao.component.html',
  styleUrls: ['./assinatura-ciencia-material-nota-fiscal-acao.component.scss']
})
export class AssinaturaCienciaMaterialNotaFiscalAcaoComponent extends ComponentFormBase  implements OnInit {

  formCienciaNf: FormGroup;
  formCienciaNfValidationHelper: FormValidationHelper;
  solicitacaoCiencia: SaidaMaterialNotaFiscalCienciaModel;
  acao:StatusAcaoMaterialNotaFiscal;
  dataSource: ItemSaidaMaterialNF[] = [];

  cienciaUsuarioRejeitado:CienciaUsuarioAprovacaoModel[];

  HistoricoProrrogacao: HistorioProrrogacaoModel[] = []

  constructor(
    private readonly formBuilder: FormBuilder,
    public dialog: MatDialog,
    private _dialogRef: MatDialogRef<any>,
    private eventMiddleService: EventMiddleService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }


  ngOnInit() {
    this.acao = StatusAcaoMaterialNotaFiscal.Saida;
    this.FormularioInitCienciaNf();
    this.ExpirouSessaoFecharModalCienciaNf()
    if(this.data.ciencia !== undefined ){
       this.ValorPadraoInitCienciaNf()
    }

  }



  private FormularioInitCienciaNf() {
      this.formCienciaNf = this.formBuilder.group({
        responsavel: [{ value: null, disabled: true }, [Validators.maxLength(20)]],
        observacao: [{ value: null, disabled: true }, [Validators.required,Validators.maxLength(25)]],
        justificativa:[{ value: null, disabled: true }, [Validators.required,Validators.maxLength(25)]],
        dataRetorno: [{ value: null, disabled: true }, Validators.required],
      });

      this.formCienciaNfValidationHelper = new FormValidationHelper({
        responsavel: 'Responsável',
        dataSaida: "Data de saída",
    });
  }



 private ValorPadraoInitCienciaNf()
  {
    this.dataSource = this.data.ciencia.solicitacaoCiencia.itens;
    this.HistoricoProrrogacao = this.data.ciencia.historicoProrrogacoes;
    this.solicitacaoCiencia = this.data.ciencia.solicitacaoCiencia;
    this.cienciaUsuarioRejeitado = this.data.ciencia.cienciaUsuarioAprovacao;
    this.formCienciaNf.get('responsavel').setValue(this.data.ciencia.solicitacaoCiencia.nomeDeUsuario)
    this.formCienciaNf.get('observacao').setValue(this.data.ciencia.solicitacaoCiencia.motivoSolicitacao)
    this.formCienciaNf.get('justificativa').setValue(this.data.ciencia.solicitacaoCiencia.justificativa)
    this.formCienciaNf.get('dataRetorno').setValue(this.data.ciencia.solicitacaoCiencia.dataProrrogacaoNF);
  }


  confirmarModal() {
    this._dialogRef.close(this.solicitacaoCiencia);
  }

  PermirtirData():boolean{
    if(this.data.ciencia.solicitacaoCiencia.idTipoCiencia == StatusCiencia.BaixaMaterialSemRetorno.valueOf()){
      return false;
    }
    return true;
  }

  cancelarModal() {
    this._dialogRef.close();
  }

  RejeitarCienciaNf(){
    const dialogJustificativa = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '285px',
      disableClose: true,
      data: {
        titulo: 'Observação para rejeição',
        confirmar: 'Confirmar rejeição'
      }
    });

    dialogJustificativa.afterClosed().subscribe(retorno => {
      if (retorno === undefined || retorno.justificativa === null) {
        return;
      }
         this.solicitacaoCiencia.observacao = retorno.justificativa;
         this.solicitacaoCiencia.ciente = true;
        this._dialogRef.close(this.solicitacaoCiencia);
    });
  }


  ExpirouSessaoFecharModalCienciaNf(){
    this.eventMiddleService.getEvent()
    .subscribe(e => {
      if(e != null && e != undefined){
        this._dialogRef.close();
      }
    });
  }
}
