import { Component, Input, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort, MatSortable } from '@angular/material';
import { BehaviorSubject } from 'rxjs';
import { AppConfig } from 'src/app/app.config';
import { IFeatureCategorias } from 'src/app/shared/models/app-config-feature-categorias.model';
import { ExibicaoDeAlertaService, TipoAlertaEnum } from '../../../core';
import { DataHelper } from '../../../helpers';
import { Grouping, MatGroupBy } from '../../../shared/mat-table-groupby/groupBy';
import { MatTableDataSource } from '../../../shared/mat-table-groupby/table-data-source';
import { PdfPreviewForDialogModel } from '../../../shared/models/pdf-preview-for-dialog.model';
import { PdfPreviewForDialogComponent } from '../../../shared/pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { AssinaturaService } from '../../assinatura.service';
import { AssinaturaStatusDocumento } from '../../enums/assinatura-status-documento-enum';
import { AssinaturaInformacoesModel, AssinaturaUploadResponseModel } from '../../models';
import { AssinaturaGerenciamentoPendenciaAssinaturaComponent } from './modal-pendencias';
import { AssinaturaGerenciamentoRepresentanteComponent } from './modal-representante/modal-representante.component';
import { CategoriaDocumento } from '../../enums/categoria-documento.enum';

@Component({
  selector: 'app-assinatura-gerenciamento-tabela',
  templateUrl: './tabela.component.html',
  styleUrls: ['./tabela.component.scss']
})
export class AssinaturaGerenciamentoTabelaComponent implements OnInit, OnChanges {
  @Input() dataSource: AssinaturaInformacoesModel[] = [];

  displayedColumns: string[] = [
    'titulo',
    'numeroDocumento',
    'statusDescricao',
    'categoriaDescricao',
    'dataCriacao',
    'acao'
  ];

  matDataSource: MatTableDataSource<AssinaturaInformacoesModel>;
  configCategorias: IFeatureCategorias[];

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataChange = new BehaviorSubject<AssinaturaInformacoesModel[]>([]);

  statusLabel: Map<string, string>;

  constructor(
    private readonly assinaturaService: AssinaturaService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    public dialog: MatDialog,
    public matGroupBy: MatGroupBy<AssinaturaInformacoesModel>
  ) { }

  ngOnInit() {
    this.configurarDataSource();

    this.statusLabel = new Map<string, string>([
      ['EmConstrucao', 'Em Construção'],
      ['NaoIniciado', 'Não Iniciado'],
      ['EmAndamento', 'Em Andamento'],
      ['Concluido', 'Concluído'],
      ['ConcluidoComRejeicao', 'Concluído com Rejeição'],
      ['Cancelado', 'Cancelado']
    ]);

    this.configCategorias = AppConfig.settings.categorias;
  }

  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'dataSource' && changes[propName].currentValue) {
        this.dataChange.next(changes[propName].currentValue);
      }
    }
  }

  aplicarFiltro(filterValue: string) {
    this.matDataSource.filter = filterValue.trim().toLowerCase();
  }

  private configurarDataSource(): void {

    this.matDataSource = new MatTableDataSource<AssinaturaInformacoesModel>();
    this.matDataSource.sort = this.sort;

    this.dataChange.subscribe(data => {
      this.matDataSource.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.matDataSource.paginator = this.paginator;
      this.matGroupBy.grouping = new Grouping();
      this.matGroupBy.grouping.groupingDataAccessor = (row) => `${row.usuarioAd.toUpperCase()}`;
      this.matDataSource.groupBy = this.matGroupBy;
    });

    this.matDataSource.sortingDataAccessor = (value: any, sortHeaderId: string): string => {
      if (sortHeaderId === 'dataCriacao' && value[sortHeaderId] !== undefined) {
        return value[sortHeaderId];
      }

      return value[sortHeaderId];
    };

    this.matDataSource.filterPredicate = (data: AssinaturaInformacoesModel, filter: string) => {
      if (
        (
          data.titulo !== null
          && data.titulo.toLowerCase().trim().indexOf(filter.toLowerCase().trim()) > -1
        )
        ||
        (
          data.numeroDocumento !== null && data.numeroDocumento !== undefined
          && data.numeroDocumento.toString().toLowerCase().trim().indexOf(filter.toLowerCase().trim()) > -1
        )
        ||
        (
          data.categoriaDescricao !== null
          && data.categoriaDescricao.toString().toLowerCase().trim().indexOf(filter.toLowerCase().trim()) > -1
        )
        ||
        (
          data.status !== null
          && data.status.toString().toLowerCase().trim().indexOf(filter.toLowerCase().trim()) > -1
        )
        ||
        (
          data.usuarioAd !== null
          && data.usuarioAd.toString().toLowerCase().trim().indexOf(filter.toLowerCase().trim()) > -1
        )
        ||
        (
          data.dataCriacao !== null
          && DataHelper.parseToPortugueseStringDate(new Date(data.dataCriacao)).indexOf(filter.toLowerCase().trim()) > -1
        )
      ) {
        return true;
      }

      return false;
    };

     this.ordenarDataSourceConfiguracaoPadrao();
  }

  ordenarDataSourceConfiguracaoPadrao() {
    this.sort.sort({
      id: 'dataCriacao',
      start: 'desc'
    } as MatSortable);
  }

  isGroup(index, item): boolean {
    return item.group;
  }

  obterDescricaoStatus(assianturaInformacao: AssinaturaInformacoesModel): string {
    return this.statusLabel.get(assianturaInformacao.status);
  }

  alterarDestaque(assinaturaInformacao: AssinaturaInformacoesModel) {
    assinaturaInformacao.destaque = !assinaturaInformacao.destaque;
    this.assinaturaService
      .atualizarDestaque(assinaturaInformacao)
      .subscribe(() => {
        this.alertSucessoAlteracaoDestaque(assinaturaInformacao);
      }, () => {
        assinaturaInformacao.destaque = !assinaturaInformacao.destaque
      });
  }

  alterarRepresentante(assianturaInformacao: AssinaturaInformacoesModel) {
    this.assinaturaService.listarPassoAssinanteRepresentanteProcesso(assianturaInformacao.id,false)
      .subscribe(passos => {

        const listaRepresentantes = [...passos].sort((a, b) => a.usuarioAdAssinate.localeCompare(b.usuarioAdAssinate));

        const dialogRef = this.dialog.open(AssinaturaGerenciamentoRepresentanteComponent, {
          disableClose: true,
          height: '768',
          width: '800px',
          data: listaRepresentantes
        });

        dialogRef.afterClosed().subscribe((sucesso) => {
          if (sucesso) {
            this.alertSucessoAtribuirRepresentantes();
          }
        });
      });
  }

  visualizarDetalhesDoProcesso(assinaturaInformacao: AssinaturaInformacoesModel) {
    this.assinaturaService.listarArquivosUploadPorPadId(assinaturaInformacao.id, true)
      .subscribe(arquivos => {
        this.dialog.open(PdfPreviewForDialogComponent, {
          disableClose: true,
          width: '920px',
          data: new PdfPreviewForDialogModel(this.obterArquivoDoProcesso(arquivos), `Documento_${assinaturaInformacao.numeroDocumento}`, 'PDF - Detalhes do documento')
        });
      });
  }

  visualizarDocumento(assinaturaInformacao: AssinaturaInformacoesModel) {
    this.assinaturaService.listarArquivosUploadPorPadId(assinaturaInformacao.id, true)
      .subscribe(arquivos => {
        this.dialog.open(PdfPreviewForDialogComponent, {
          disableClose: true,
          width: '920px',
          data: new PdfPreviewForDialogModel(this.arquivoAtualDoProcesso(arquivos), `Documento_${assinaturaInformacao.numeroDocumento}`, 'PDF - Detalhes do documento')
        });
      });
  }

  obterArquivoDoProcesso(arquivos: AssinaturaUploadResponseModel[]) {
    const arquivoFinal = arquivos.filter(arquivo => arquivo.arquivoFinal && arquivo.status).sort(arquivos => arquivos.atualizacao).reverse();

    if (arquivoFinal.length > 0) {
      return arquivoFinal[0].arquivoBinario;
    }

    return this.arquivoAtualDoProcesso(arquivos);
  }

  private arquivoAtualDoProcesso(arquivos: AssinaturaUploadResponseModel[]) {
    const arquivoAtualProcesso = arquivos.filter(arquivo => !arquivo.arquivoFinal).sort(arquivo => arquivo.ordem).reverse();

    return arquivoAtualProcesso[0].arquivoBinario;
  }

  private alertSucessoAtribuirRepresentantes() {
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: `Representantes foram atribuídos com suceso!`,
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    });
  }

  obterProcessoComPassosParaAssinatura(assianturaInformacao: AssinaturaInformacoesModel) {
    this.assinaturaService.obterProcessoComPassosParaAssinatura(assianturaInformacao.id)
      .subscribe(processo => {
        this.dialog.open(AssinaturaGerenciamentoPendenciaAssinaturaComponent, {
          disableClose: true,
          height: '768',
          width: '800px',
          data: processo
        });
      });
  }

  verificarProcessoEstaAtivo(statusParam: string): boolean {
    const status = AssinaturaStatusDocumento[statusParam];
    return (
      status === AssinaturaStatusDocumento.Emconstrucao ||
      status === AssinaturaStatusDocumento.NaoIniciado ||
      status === AssinaturaStatusDocumento.EmAndamento
    );
  }

  verificarStatusPagamentoSolicitacaoSaida(assianturaInformacao: AssinaturaInformacoesModel): boolean {
    const categoria = assianturaInformacao.categoriaId;
    return (
      categoria === CategoriaDocumento.Statuspagamento ||
      categoria === CategoriaDocumento.SaidadeMaterialComNotaFiscal ||
      categoria === CategoriaDocumento.AutorizacaodeSaidadeMateriais ||
      categoria === CategoriaDocumento.AutorizacaodeSaidadeMaterialCOMRetorno ||
      categoria === CategoriaDocumento.AutorizacaodeSaidadeMaterialSEMRetorno
    );
  }


  verificaStatusConcluido(statusParam: string): boolean {
    const status = AssinaturaStatusDocumento[statusParam];
    return (
      status === AssinaturaStatusDocumento.Concluido
    );
  }

  confirmarCancelamento(assianturaInformacao: AssinaturaInformacoesModel): void {
    this.exibicaoDeAlertaService
      .exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja cancelar o processo de aprovação ${assianturaInformacao.titulo}?`)
      .then((resposta) => {
        if (resposta.value) {
          this.assinaturaService
            .cancelarProcesso(assianturaInformacao)
            .subscribe((processoCancelado) => {
              const processoGrid = this.dataSource.filter(
                (processo) => processo.id === processoCancelado.id
              )[0];
              if (processoGrid) {
                processoGrid.status = processoCancelado.status;
                this.alertSucessoCancelamento(assianturaInformacao);
              }
            });
        }
      });
  }

  private alertSucessoCancelamento(assianturaInformacao: AssinaturaInformacoesModel) {
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: `Processo de aprovação ${assianturaInformacao.titulo} foi cancelado com sucesso!`,
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    });
  }

  private alertSucessoAlteracaoDestaque(assianturaInformacao: AssinaturaInformacoesModel) {
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: `Processo de aprovação ${assianturaInformacao.titulo} foi atualizado com sucesso!`,
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    });
  }
}
