import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ComponentFormBase } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { SolicitacaoCienciaModel } from '../../../models/solicitacao-ciencia-model';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { CienciaUsuarioAprovacaoModel } from 'src/app/saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';
import { DocumentoFI1548CienciaModel } from 'src/app/assinatura/models/documento-fi1548-ciencia-model';

@Component({
  selector: 'app-assinatura-ciencia-status-pagamento-acao',
  templateUrl: './assinatura-ciencia-status-pagamento-acao.component.html',
  styleUrls: ['./assinatura-ciencia-status-pagamento-acao.component.scss']
})
export class AssinaturaCienciaStatusPagamentoAcaoComponent extends ComponentFormBase  implements OnInit {

  form: FormGroup;
  formValidationHelper: FormValidationHelper;
  solicitacaoCiencia: SolicitacaoCienciaModel;
  cienciaRejeitado:CienciaUsuarioAprovacaoModel[];
  dataSource: DocumentoFI1548CienciaModel;

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
        fornecedor: [{ value: null, disabled: true }, Validators.required],
        valorTotal: [{ value: null, disabled: true }, Validators.required],
        pagamento: [{ value: null, disabled: true }, Validators.required],
      });

      this.formValidationHelper = new FormValidationHelper({
        responsavel: 'Responsável',
        dataSaida: "Data de saída",
    });
  }



 private PreencherFormularioEdicao()
  {
    this.dataSource = this.data.ciencia;
    this.cienciaRejeitado = this.data.ciencia.cienciaUsuarioAprovacao;
    this.form.get('responsavel').setValue(this.data.ciencia.nomeUsuario)
    this.form.get('observacao').setValue(this.data.ciencia.motivoSolicitacao)
    this.form.get('justificativa').setValue(this.data.ciencia.justificativa)
    this.form.get('fornecedor').setValue(this.data.ciencia.fornecedor)
    this.form.get('valorTotal').setValue(this.data.ciencia.valorTotal)
    this.form.get('pagamento').setValue(this.data.ciencia.pagamento)
  }


  confirmarModal() {
    this._dialogRef.close(this.dataSource);
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
        this.dataSource.motivoRejeicao = retorno.justificativa;
        this.dataSource.ciente = true;
        this._dialogRef.close(this.dataSource);
    });
  }

}
