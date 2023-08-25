import { HistorioProrrogacaoModel } from './../../../FI347/shared/models/historico-prorrogacao.model';
import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from 'src/app/core';
import { StatusAcaoMaterial } from 'src/app/FI347/shared/enums/status-acao-materia';
import { ItemSaidaMateialModel } from 'src/app/FI347/shared/models/item-saida-material.model';
import { RegistroSaidaMaterial } from 'src/app/FI347/shared/models/registro-saida-material';
import { FormValidationHelper } from 'src/app/helpers';
import { SolicitacaoCienciaModel } from '../../models/solicitacao-ciencia-model';
import { StatusCiencia } from 'src/app/FI347/shared/enums/status-ciencia';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { CienciaUsuarioAprovacaoModel } from 'src/app/saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';

@Component({
  selector: 'app-assinatura-ciencia-acao',
  templateUrl: './assinatura-ciencia-acao.component.html',
  styleUrls: ['./assinatura-ciencia-acao.component.scss']
})
export class AssinaturaCienciaAcaoComponent extends ComponentFormBase  implements OnInit {



  form: FormGroup;
  formValidationHelper: FormValidationHelper;
  solicitacaoCiencia: SolicitacaoCienciaModel;
  acao:StatusAcaoMaterial;
  RegistroSaida: RegistroSaidaMaterial;
  dataSource: ItemSaidaMateialModel[] = [];
  cienciaRejeitado:CienciaUsuarioAprovacaoModel[];

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
    this.acao = StatusAcaoMaterial.RegistroSaida;
    this.FormularioInitAcao();
    this.ExpirouSessaoFecharModalCiencia();
    if(this.data.ciencia !== undefined ){
       this.PreencherFormularioEdicao()
    }

  }

  ExpirouSessaoFecharModalCiencia(){
    this.eventMiddleService.getEvent()
    .subscribe(event => {
      if(event != null && event != undefined){
        this._dialogRef.close();
      }
    });
  }

  private FormularioInitAcao() {
      this.form = this.formBuilder.group({
        responsavel: [{ value: null, disabled: true }, [Validators.maxLength(20)]],
        observacao: [{ value: null, disabled: true }, [Validators.required,Validators.maxLength(25)]],
        justificativa:[{ value: null, disabled: true }, [Validators.required,Validators.maxLength(25)]],
        dataRetorno: [{ value: null, disabled: true }, Validators.required],
      });

      this.formValidationHelper = new FormValidationHelper({
        responsavel: 'Responsável',
        dataSaida: "Data de saída",
    });
  }



 private PreencherFormularioEdicao()
  {
    this.dataSource = this.data.ciencia.solicitacaoCiencia.itens;
    this.cienciaRejeitado = this.data.ciencia.cienciaUsuarioAprovacao;
    this.HistoricoProrrogacao = this.data.ciencia.historicoProrrogacoes;
    this.solicitacaoCiencia = this.data.ciencia.solicitacaoCiencia;


    this.form.get('responsavel').setValue(this.data.ciencia.solicitacaoCiencia.nomeUsuario)
    this.form.get('observacao').setValue(this.data.ciencia.solicitacaoCiencia.motivoSolicitacao)
    this.form.get('justificativa').setValue(this.data.ciencia.solicitacaoCiencia.justificativa)
    this.form.get('dataRetorno').setValue(this.data.ciencia.solicitacaoCiencia.dataProrrogacao)
  }


  confirmarModal() {
    this._dialogRef.close(this.solicitacaoCiencia);
  }

  PermirtirCampo():boolean{
    if(this.data.ciencia.solicitacaoCiencia.idTipoCiencia == StatusCiencia.BaixaMaterialSemRetorno.valueOf()){
      return false;
    }
    return true;
  }

  cancelarModal() {
    this._dialogRef.close();
  }

  RejeitarCiencia(){
    const dialogJustificativa = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '285px',
      disableClose: true,
      data: {
        titulo: 'Observação para rejeição',
        confirmar: 'Confirmar rejeição',
        maxLength: 200
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

}
