import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material';
import { ComponentFormBase, ExibicaoDeAlertaService } from '../../core';
import { FormValidationHelper } from '../../helpers';
import { PdfPreviewForDialogModel } from '../../shared/models/pdf-preview-for-dialog.model';
import { PdfPreviewForDialogComponent } from '../../shared/pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { UsuarioModel } from '../../usuario/models';
import { UsuarioService } from '../../usuario/usuario.service';
import { Fi1548Service } from '../FI1548.service';
import { DocumentoModel, TipoPagamentoModel } from '../shared';
import { StatusDocumento } from '../shared/models/status-documento.model';

@Component({
  selector: 'app-consulta',
  templateUrl: './consulta.component.html',
  styleUrls: ['./consulta.component.scss']
})
export class ConsultaComponent extends ComponentFormBase implements OnInit {
  dataSource: DocumentoModel[];
  formPesquisar: FormGroup;
  formValidationHelper: FormValidationHelper;
  tiposPagamento: TipoPagamentoModel[];
  autores: UsuarioModel[];
  statusList: StatusDocumento[];
  filtroParaRdl: any;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly usuarioService: UsuarioService,
    private readonly fi1548Service: Fi1548Service,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.carregarFormPesquisar();
  }

  formPesquisarOnSubmit(): void {
    if (!this.validarFormPesquisar()) {
      return;
    }

    this.dataSource = [];
    this.filtroParaRdl = this.formPesquisar.value;

    this.fi1548Service.listarDocumentos(this.formPesquisar.value)
      .subscribe(
        response => {
          this.dataSource = response;
        }
      );
  }

  gerarRdl(): void {
    this.formPesquisar.value.ordenar = JSON.parse(localStorage.getItem('ordenacaoStatusPagamento'));
    this.fi1548Service.rdlToPdfBytesConverter(this.filtroParaRdl)
      .subscribe(
        binarioBase64 => {
          this.dialog.open(PdfPreviewForDialogComponent, {
            disableClose: true,
            width: '920px',
            data: new PdfPreviewForDialogModel(binarioBase64, `ICE FI 2349 – Relatório de Status de Pagamentos`, 'ICE FI 2349 – Relatório de Status de Pagamentos')
          });
        }
      );
  }

  cancelarEventEmitter(cancelou: boolean): void {
    if (cancelou) {
      this.formPesquisarOnSubmit();
    }
  }

  private carregarFormPesquisar(): void {
    this.formPesquisar = this.formBuilder.group({
      numero: null,
      tiposPagamento: new FormControl([]),
      autoresADId: new FormControl([]),
      status: new FormControl([]),
      dataInicio: null,
      dataTermino: null
    });

    this.formValidationHelper = new FormValidationHelper({
      numero: 'Número',
      autoresADId: 'Autor',
      tiposPagamento: 'Tipo de pagamento',
      status: 'Status do Documento'
    });

    this.carregarComboTiposPagamento();
    this.carregarComboAutores();
    this.carregarComboStatusDocumento();
  }



  private carregarComboTiposPagamento(): void {
    this.fi1548Service.listarTiposPagamento()
      .subscribe(
        response => {
          this.tiposPagamento = response;
        },
        error => {
          console.error('Erro no método fi1548Service.listarTiposPagamento.', error);
        }
      );
  }

  private carregarComboStatusDocumento() {
    this.fi1548Service.listarStatusDocumento()
      .subscribe(
        response => {
          this.statusList = response;
        },
        error => {
          console.error('Erro no método fi1548Service.listarTiposPagamento.', error);
        }
      );
  }

  private carregarComboAutores(): void {
    this.usuarioService.listarUsuarios(0, '')
      .subscribe(
        response => {
          this.autores = this.ordenarAutoresPorNome(response);
        },
        error => {
          console.error('Erro no método usuarioService.listarUsuarios.', error);
        }
      );
  }

  private ordenarAutoresPorNome(usuarioList: UsuarioModel[]): UsuarioModel[] {
    if (usuarioList) {
      return usuarioList.sort((a, b) => a.nome.localeCompare(b.nome));
    }
    return usuarioList;
  }

  resetFormPesquisar(): void {
    this.formPesquisar.reset();
    this.carregarFormPesquisar();
    this.dataSource = null;
  }

  private validarFormPesquisar(): boolean {
    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.formPesquisar.controls);

    if (this.formPesquisar.controls['dataInicio'].value !== null
      && this.formPesquisar.controls['dataTermino'].value !== null) {
      const dtInicial = this.formPesquisar.controls['dataInicio'].value as Date;
      const dtFinal = this.formPesquisar.controls['dataTermino'].value as Date;

      if (dtInicial > dtFinal) {
        errorList.push('Data Término deve ser igual ou maior que a Data Início.');
      }
    }

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }
}
