import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { concat, Observable, of, Subject } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { AutenticacaoService } from 'src/app/autenticacao';
import { ComponentFormBase, ExibicaoDeAlertaService } from 'src/app/core';
import { FormValidationHelper } from 'src/app/helpers';
import { PdfPreviewForDialogModel } from 'src/app/shared/models/pdf-preview-for-dialog.model';
import { PdfPreviewForDialogComponent } from 'src/app/shared/pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { UsuarioModel } from 'src/app/usuario/models';
import { UsuarioService } from 'src/app/usuario/usuario.service';
import { AssinaturaService } from '../assinatura.service';
import { AssinaturaCategoriaModel, AssinaturaInformacoesModel } from '../models';
import { AssinaturaInformacoesStatusDocumento } from '../models/assinatura-informacaoes-status-documento.model';
import { AssinaturaGerenciamentoTabelaComponent } from './tabela/tabela.component';

@Component({
  selector: 'app-assinatura-gerenciamento',
  templateUrl: './assinatura-gerenciamento.component.html',
  styleUrls: ['./assinatura-gerenciamento.component.scss']
})
export class AssinaturaGerenciamentoComponent extends ComponentFormBase implements OnInit {
  
  @ViewChild('compTabela') public compTabela: AssinaturaGerenciamentoTabelaComponent;
  dataSource: AssinaturaInformacoesModel[];
  formPesquisarAssinatura: FormGroup;
  formValidationHelper: FormValidationHelper;
  autores: UsuarioModel[];
  statusAssinaturaList: AssinaturaInformacoesStatusDocumento[];
  obsListaNomes$: Observable<string[]>;
  listaNomesinput$ = new Subject<string>();
  listaNomesLoading = false;
  categorias: AssinaturaCategoriaModel[];

  constructor(
    private readonly autenticacaoService: AutenticacaoService,
    private readonly assinaturaService: AssinaturaService,
    private readonly usuarioService: UsuarioService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.carregarFormPesquisar();
    this.carregarNomesDocumento();
  }

  private retornarFiltros(): any {
    let filtro = this.formPesquisarAssinatura.value;

    if (this.compTabela !== undefined) {
      let objOrdenacao = new Object();
      objOrdenacao["direcao"] = this.compTabela.sort.direction;
      objOrdenacao["campo"] = this.compTabela.sort.active;
      filtro["ordenacao"] = objOrdenacao;
    }

    return filtro;
  }

  formPesquisarAssinaturaOnSubmit(): void {
    if (!this.validarFormPesquisarAssinatura()) {
      return;
    }

    this.assinaturaService.listarGerenciamento(this.retornarFiltros())
      .subscribe(
        response => {
          this.dataSource = response;
        }
      );
  }

  private carregarFormPesquisar(): void {
    this.formPesquisarAssinatura = this.formBuilder.group({
      numeroDocumento: null,
      nomeDocumento: null,
      usuariosGuidAd: new FormControl([]),
      status: new FormControl([]),
      categoriaId: new FormControl([]),
      dataInicio: null,
      dataTermino: null
    });

    this.formValidationHelper = new FormValidationHelper({
      numeroDocumento: 'Número do documento',
      nomeDocumento: 'Nome do documento',
      status: 'Status do Documento'
    });

    this.carregarComboAutores();
    this.carregarComboStatusDocumento();
    this.carregarComboCategorias();
  }

  resetFormPesquisarAssinatura(): void {
    this.formPesquisarAssinatura.reset();
    this.carregarFormPesquisar();
    this.dataSource = null;
  }

  private carregarComboAutores(): void {
    this.usuarioService.listarUsuarios(0, '')
      .subscribe(
        response => {
          this.autores = this.carregarAutoresPermitidos(response);
        },
        error => {
          console.error('Erro no método usuarioService.listarUsuarios.', error);
        }
      );
  }

  private carregarAutoresPermitidos(usuarios: UsuarioModel[]): UsuarioModel[] {
    if (this.autenticacaoService.validarPermissao('assinatura:gerenciardocumentos:todosusuarios')) {
      return usuarios;
    }

    return usuarios.filter(u => u.activeDirectoryId === this.autenticacaoService.obterDadosDeAcesso().userData.id);
  }

  private carregarComboStatusDocumento() {
    this.assinaturaService.listarStatusDocumento()
      .subscribe(
        response => {
          this.statusAssinaturaList = response;
        },
        error => {
          console.error('Erro no método fi1548Service.listarTiposPagamento.', error);
        }
      );
  }

  private carregarComboCategorias(): void {
    this.assinaturaService.listarAssinaturaCategoriaExclude(false)
      .subscribe(
        resposta => {
          this.categorias = resposta;
        },
        error => {
          console.error('Erro no método carregar categorias.', error);
        }
      );
  }

  private carregarNomesDocumento() {
    this.obsListaNomes$ = concat(
      of([]),
      this.listaNomesinput$.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => this.listaNomesLoading = true),
        switchMap(term => {
          if (term === undefined || term === null || term.trim().length < 3) {
            this.listaNomesLoading = false;
            return of([]);
          }

          return this.assinaturaService.listarNomesDocumentos(term.trim())
            .pipe(
              map(event => event.map(n => n.nome)),
              catchError(() => of([])),
              tap(() => this.listaNomesLoading = false)
            );
        })
      )
    );
  }

  private validarFormPesquisarAssinatura(): boolean {
    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.formPesquisarAssinatura.controls);

    if (this.formPesquisarAssinatura.controls['dataInicio'].value !== null && this.formPesquisarAssinatura.controls['dataTermino'].value !== null) {
      const dtInicial = this.formPesquisarAssinatura.controls['dataInicio'].value as Date;
      const dtFinal = this.formPesquisarAssinatura.controls['dataTermino'].value as Date;
      if (dtInicial > dtFinal) {
        errorList.push('Data Término deve ser igual ou maior que a Data Início.');
      }
    }
    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
    }

    return errorList.length === 0;
  }

  gerarRdl(): void {
    this.assinaturaService.rdlToPdfBytesConverter(this.retornarFiltros())
      .subscribe(
        binarioBase64 => {
          this.dialog.open(PdfPreviewForDialogComponent, {
            disableClose: true,
            width: '920px',
            data: new PdfPreviewForDialogModel(binarioBase64, `ICE FI 2377 – Relatório de Consulta de Documentos`, 'ICE FI 2377 – Relatório de Consulta de Documentos')
          });
        }
      );
  }
}
