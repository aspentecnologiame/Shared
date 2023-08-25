import { UsuarioModel } from './../../usuario/models/usuario.model';
import { DropDownModel } from 'src/app/shared/models/dropdown.model';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatCheckboxChange, MatDialog } from '@angular/material';
import { ComponentFormBase, ExibicaoDeAlertaService } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { UsuarioService } from 'src/app/usuario/usuario.service';
import { SaidaMaterialNotaFiscalService } from '../saida-material-nota-fiscal.service';
import { SaidaMaterialNotaFiscal } from '../shared/models/SaidaMaterialNotaFiscal';
import { PdfPreviewForDialogComponent } from 'src/app/shared/pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { PdfPreviewForDialogModel } from 'src/app/shared/models/pdf-preview-for-dialog.model';
import { StatusSaidaMaterialNF as EnumStatusSaidaMaterialNF } from '../shared/enums/StatusSaidaMaterialNF';
import { uniqueId } from 'lodash';

@Component({
  selector: 'app-consulta',
  templateUrl: './consulta.component.html'
})
export class ConsultaComponent extends ComponentFormBase implements OnInit {

  formPesquisarMaterialNF: FormGroup;
  formMaterialNFHelper: FormValidationHelper;
  dataSource: SaidaMaterialNotaFiscal[] = [];
  listaTiposSaidaNF: DropDownModel[];
  listaUsuariosNF: UsuarioModel[];
  listaStatusNF: DropDownModel[];
  listaStatusFiltro: DropDownModel[];
  backupStatusNF: Number[];
  filtroStatusPadrao = [];

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly usuarioService: UsuarioService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly service: SaidaMaterialNotaFiscalService,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit() {
    this.tipoSaidaInit();
    this.ListarUsuariosDropdownInit();
    this.ListarStatusDropdownInit();
    this.carregarFormularioMaterial();
    this.ListarStatusDropdownFiltro()
    super.ngOnInit();
  }

  private tipoSaidaInit() {
    this.service.listarTipoSaidaMaterialNF().subscribe((listaTipoSaidaNF) => {
      this.listaTiposSaidaNF = listaTipoSaidaNF;
    });
  }

  private ListarStatusDropdownFiltro() {
    this.service.listarStatusFiltroPadrao().subscribe((listaDeStatusFiltro) => {
      this.listaStatusFiltro = listaDeStatusFiltro;

      if (this.listaStatusFiltro.length > 0) {

        this.listaStatusFiltro.forEach((filtroAtual) => {
          this.filtroStatusPadrao.push(filtroAtual.id);
        })

        this.formPesquisarMaterialNF.get('status').setValue(this.filtroStatusPadrao);
        this.pesquisarSaidaMaterialNFSubmit();
      }
    });


  }

  private ListarStatusDropdownInit() {
    this.service.listarStatusNotaFiscal().subscribe((listaDeStatus) => {
      this.listaStatusNF = listaDeStatus;
    });
  }


  private ListarUsuariosDropdownInit(): void {
    this.usuarioService.listarUsuarios(0, '')
      .subscribe(usuarios => {
        this.listaUsuariosNF = this.ordenarUsuariosPorNome(usuarios);
      },
        error => {
          console.error('Erro no método usuarioService.listarUsuarios.', error);
        }
      );
  }

  private ordenarUsuariosPorNome(listaUsuario: UsuarioModel[]): UsuarioModel[] {
    if (listaUsuario) {
      return listaUsuario.sort((a, b) => a.nome.localeCompare(b.nome));
    }
    return listaUsuario;
  }

  private carregarFormularioMaterial() {
    this.formMaterialNFHelper = new FormValidationHelper({
      tiposSaida: "Tipo de saída",
      autores: "Responsável",
      status: "Status do documento",
      numero: "Número da autorização de saída",
      vencido: "Vencimento",
    });


    this.formPesquisarMaterialNF = this.formBuilder.group({
      tiposSaida: new FormControl([]),
      status: new FormControl([]),
      autores: new FormControl([]),
      numero: null,
      vencido: null,
      dataInicio: null,
      dataTermino: null
    });

    this.dataSource = null;
  }


  cancelarEventEmitter(cancelou: boolean): void {
    if (cancelou) {
      this.pesquisarSaidaMaterialNFSubmit();
    }
  }

  habilitarDesabilitarStatusNF(event: MatCheckboxChange): void {

    const customListNF: Number[] = [];

    if (event.checked) {
      let statusListNF = this.formPesquisarMaterialNF.controls['status'].value as Number[];
      statusListNF.forEach(element => {
        if (element !== Number(EnumStatusSaidaMaterialNF.Cancelada) && element !== Number(EnumStatusSaidaMaterialNF.Concluido))
          customListNF.push(Number(element));
      });
      this.backupStatusNF = statusListNF;
      this.formPesquisarMaterialNF.controls['status'].setValue(customListNF);
    } else {
      this.formPesquisarMaterialNF.controls['status'].setValue(this.backupStatusNF);
    }

  }

  pesquisarSaidaMaterialNFSubmit() {

    this.dataSource = [];

    this.service.consultaMaterialFiltro(this.formPesquisarMaterialNF.value)
      .subscribe(result => {
        if (result != null)
          this.dataSource = result;
      })


  }

  Limpar(): void {
    this.formPesquisarMaterialNF.reset();
    this.carregarFormularioMaterial();
  }

  gerarRel(): void {
    this.formPesquisarMaterialNF.value.exibirNaOrdem = JSON.parse(localStorage.getItem('ordenacaoSaidaMaterialNF'));
    this.service.rdlToPdfBytesConverter(this.formPesquisarMaterialNF.value)
      .subscribe(
        binarioBase64 => {
          this.dialog.open(PdfPreviewForDialogComponent, {
            disableClose: true,
            width: '920px',
            data: new PdfPreviewForDialogModel(binarioBase64, `ICE FI 2405 – Saída de Material - com NF`, 'ICE FI 2405 – Saída de Material - com NF')
          });
        }
      );
  }
}