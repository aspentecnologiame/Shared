import { SaidaMaterialModel } from 'src/app/FI347/shared/models/saida-material.model';
import { TipoSaida } from './../shared/models/tipo-saida.model';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { ComponentFormBase, ExibicaoDeAlertaService } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { Fi1347Service } from '../FI347.service';
import { UsuarioModel } from 'src/app/usuario/models';
import { UsuarioService } from 'src/app/usuario/usuario.service';
import { StatusSaidaMaterial } from '../shared/models/status-saida-material';
import { StatusSaidaMaterial as EnumStatusSaidaMaterial } from '../shared/enums/status-saida-material';
import { PdfPreviewForDialogModel } from 'src/app/shared/models/pdf-preview-for-dialog.model';
import { PdfPreviewForDialogComponent } from 'src/app/shared/pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { MatCheckboxChange, MatDialog } from '@angular/material';

@Component({
  selector: 'app-consulta',
  templateUrl: './consulta.component.html'
})
export class ConsultaComponent extends ComponentFormBase implements OnInit {
  formPesquisarMaterial: FormGroup;
  formHelper: FormValidationHelper;
  dataSource:SaidaMaterialModel[] = [];

  listaResponsavel: UsuarioModel[];
  listaTiposSaida: TipoSaida[];
  listaStatus:StatusSaidaMaterial[];
  backupStatus: Number[];

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly usuarioService: UsuarioService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly service: Fi1347Service,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.tipoSaidaInit();
    this.ResponsavelDropdownInit();
    this.comboStatusSaidaMaterial();
    this.carregarFormularioMaterial();
}

Limpar(): void {
  this.formPesquisarMaterial.reset();
  this.carregarFormularioMaterial();
}

  private validarFormularioMaterial(): boolean {
    const errorList: Array<string> = this.formHelper.getFormValidationErrorsMessages(this.formPesquisarMaterial.controls);

    if (this.formPesquisarMaterial.controls['dataInicio'].value !== null && this.formPesquisarMaterial.controls['dataTermino'].value !== null) {
      const datatInicial = this.formPesquisarMaterial.controls['dataInicio'].value as Date;
      const dataFinal = this.formPesquisarMaterial.controls['dataTermino'].value as Date;

      if (datatInicial > dataFinal) {
        errorList.push('Data Término deve ser igual e/ou maior que a Data Início.');
      }
    }

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }

  private carregarFormularioMaterial()
  {
    this.formHelper = new FormValidationHelper({
      tiposSaida:"Tipo de saída",
      autores:"Responsável",
      status:"Status do documento",
      numero:"Número da autorização de saída",
      patrimonio:"Patrimonio",
      vencido:"Vencimento",
    });


    this.formPesquisarMaterial = this.formBuilder.group({
      tiposSaida: new FormControl([]),
      status: new FormControl([]),
      autores: new FormControl([]),
      numero:null,
      patrimonio:null,
      vencido:null,
      dataInicio: null,
      dataTermino: null
    });

    this.dataSource = null;
  }

private tipoSaidaInit () {
  this.service.listarTipoSaida().subscribe((saida) => {
    this.listaTiposSaida = saida;
  });
}

private comboStatusSaidaMaterial () {
  this.service.listarStatusSaida().subscribe((status) => {
    this.listaStatus = status;
  });
}


private ResponsavelDropdownInit(): void {
  this.usuarioService.listarUsuarios(0, '')
    .subscribe(response => {
      this.listaResponsavel = this.ordenarResponsavelPorNome(response);
      },
      error => {
        console.error('Erro no método usuarioService.listarUsuarios.', error);
      }
    );
}

private ordenarResponsavelPorNome(usuarioList: UsuarioModel[]): UsuarioModel[] {
    if (usuarioList) {
      return usuarioList.sort((a, b) => a.nome.localeCompare(b.nome));
    }
    return usuarioList;
}

habilitarDesabilitarStatus(event: MatCheckboxChange): void {

  const customList: Number[] = [];

  if (event.checked) {
    let statusList = this.formPesquisarMaterial.controls['status'].value as Number[];
    statusList.forEach(element => {
      if (element !== Number(EnumStatusSaidaMaterial.Cancelado) && element !== Number(EnumStatusSaidaMaterial.Concluido))
        customList.push(Number(element));
    });
    this.backupStatus = statusList;
    this.formPesquisarMaterial.controls['status'].setValue(customList);
  } else {
    this.formPesquisarMaterial.controls['status'].setValue(this.backupStatus);
  }

}

cancelarEventEmitter(cancelou: boolean): void {
  if (cancelou) {
    this.pesquisarSaidaMaterialSubmit();
  }
}

pesquisarSaidaMaterialSubmit(): void {
  if (!this.validarFormularioMaterial()) {
    return;
  }

  this.dataSource = [];
  this.service.listarSaidaMaterial(this.formPesquisarMaterial.value)
     .subscribe(
       response => {
         this.dataSource = response;
       }
  );
}

gerarRdl(): void {
    this.formPesquisarMaterial.value.ordenacao = JSON.parse(localStorage.getItem('ordenacaoSaidaMaterial'));
    this.service.rdlToPdfBytesConverter(this.formPesquisarMaterial.value)
      .subscribe(
        binarioBase64 => {
          this.dialog.open(PdfPreviewForDialogComponent, {
            disableClose: true,
            width: '920px',
            data: new PdfPreviewForDialogModel(binarioBase64, `ICE FI 2396 – Autorizações de Saída de Material - sem NF`, 'ICE FI 2396 – Autorizações de Saída de Material - sem NF')
          });
        }
      );
  }
}
