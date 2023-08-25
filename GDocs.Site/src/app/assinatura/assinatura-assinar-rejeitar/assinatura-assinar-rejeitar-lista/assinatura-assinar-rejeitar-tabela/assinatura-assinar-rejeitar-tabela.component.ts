import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatCheckboxChange, MatDialog, MatPaginator, MatSort, MatTableDataSource, Sort } from '@angular/material';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AssinaturaGerenciamentoPendenciaAssinaturaComponent } from 'src/app/assinatura/assinatura-gerenciamento/tabela/modal-pendencias';
import { AssinaturaAbas } from 'src/app/assinatura/enums/assinatura-abas-enum';
import { CategoriaDocumento } from 'src/app/assinatura/enums/categoria-documento.enum';
import { DocumentoFI1548CienciaModel } from 'src/app/assinatura/models/documento-fi1548-ciencia-model';
import { SolicitacaoCienciaModel } from 'src/app/assinatura/models/solicitacao-ciencia-model';
import { Fi1548Service } from 'src/app/FI1548';
import { Fi1347Service } from 'src/app/FI347/FI347.service';
import { StatusAcaoMaterial } from 'src/app/FI347/shared/enums/status-acao-materia';
import { SaidaMaterialModel } from 'src/app/FI347/shared/models/saida-material.model';
import { SaidaMaterialNotaFiscalService } from 'src/app/saida-material-nota-fiscal/saida-material-nota-fiscal.service';
import { CienciaEhHistoricoResponseModel } from 'src/app/saida-material-nota-fiscal/shared/models/CienciaEhHistoricoResponseModel';
import { SaidaMaterialNotaFiscalCienciaModel } from 'src/app/saida-material-nota-fiscal/shared/models/saida-material-nota-fiscal-ciencia-model';
import { PdfPreviewForDialogModel } from 'src/app/shared/models/pdf-preview-for-dialog.model';
import { PdfPreviewForDialogComponent } from 'src/app/shared/pdf-preview-for-dialog/pdf-preview-for-dialog.component';
import { AutenticacaoService } from '../../../../autenticacao';
import { FeatureTogglesService } from '../../../../core';
import { FeatureToggles } from '../../../../core/enums/feature-toggles-enum';
import { ExibicaoDeAlertaService } from '../../../../core/services/exibicao-de-alerta/exibicao-de-alerta.service';
import { DataHelper } from '../../../../helpers';
import { AssinaturaService } from '../../../assinatura.service';
import { AssinaturaStatusPassoAtual, AssinaturaStatusPassoAtualLabel } from '../../../enums/assinatura-status-passo-atual-enum';
import { AssinaturaPassoItemAssinarRejeitarModel, DocumentoPendenteAssinaturaModel } from '../../../models';
import { AssinaturaCienciaAcaoComponent } from '../../assinatura-ciencia-acao/assinatura-ciencia-acao.component';
import { AssinaturaCienciaMaterialNotaFiscalAcaoComponent } from '../../assinatura-ciencia-acao/assinatura-ciencia-material-nota-fiscal-acao/assinatura-ciencia-material-nota-fiscal-acao.component';
import { AssinaturaCienciaStatusPagamentoAcaoComponent } from '../../assinatura-ciencia-acao/assinatura-ciencia-status-pagamento-acao/assinatura-ciencia-status-pagamento-acao.component';


@Component({
  selector: 'app-assinatura-assinar-rejeitar-tabela',
  templateUrl: './assinatura-assinar-rejeitar-tabela.component.html',
  styleUrls: ['./assinatura-assinar-rejeitar-tabela.component.scss']
})
export class AssinaturaAssinarRejeitarTabelaComponent implements OnInit, OnChanges {


  @Input() dataSource: DocumentoPendenteAssinaturaModel[] = [];
  @Input() documentosSelecionadosLista: number[] = [];
  @Input() selectedTabIndex: number;

  @Output() eventAtualizarMatriz = new EventEmitter();
  @Output() AtualizarTabela = new EventEmitter();
  @Output() PararContador = new EventEmitter<boolean>();

  @Output() eventDocumentosSelecionados = new EventEmitter<number[]>();

  AssinaturaStatusPassoAtual: AssinaturaStatusPassoAtual;

  saidaMaterial:SaidaMaterialModel;
  documentosSelecionados: number[] = [];
  selecionarTodos: boolean;
  displayedColumns: string[] = [];

  matDataSource: MatTableDataSource<DocumentoPendenteAssinaturaModel>;
  tituloCiencia:string = 'Ciência de Solicitações - ';
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) matSort: MatSort;
  sort: Sort = {
    active: 'dataCriacao',
    direction: 'asc',
  };

  dataChange = new BehaviorSubject<DocumentoPendenteAssinaturaModel[]>([]);
  abaCiencia: number;

  constructor(
    private readonly fi1347Service: Fi1347Service,
    private readonly serviceMaterialNF: SaidaMaterialNotaFiscalService,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly assinaturaService: AssinaturaService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly router: Router,
    public dialog: MatDialog,
    private readonly featureTogglesService: FeatureTogglesService,
    private readonly fi1548Service: Fi1548Service
  ) { }

  ngOnInit() {
    this.configurarDisplayColumns();
    this.configurarDataSource();
    this.selecionarDocumentosLista();
  }

  aplicarFiltro(filterValue: string) {
    this.matDataSource.filter = filterValue.trim().toLowerCase();
  }

  private configurarDisplayColumns(): void {

    this.abaCiencia = AssinaturaAbas.Ciencia;

    this.displayedColumns = [
      "numero",
      "descricao",
      "observacao",
      "descricaoCategoria",
      "dataCriacao",
      "acao",
    ];

    if (this.autenticacaoService.validarPermissao('assinatura:pendencias:assinaremlote')) {
      this.displayedColumns.splice(0, 0, "selecionado");
    }
  }

  private configurarDataSource(): void {
    this.matDataSource = new MatTableDataSource<DocumentoPendenteAssinaturaModel>();

    this.dataChange.subscribe((data) => {
      this.matDataSource.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.matDataSource.paginator = this.paginator;
    });

    this.matDataSource.sortingDataAccessor = (
      value: any,
      sortHeaderId: string
    ): string => {
      if (sortHeaderId === "data" && value[sortHeaderId] !== undefined) {
        return value[sortHeaderId].toLocaleLowerCase();
      }

      if (typeof value[sortHeaderId] === "string") {
        return value[sortHeaderId].toLocaleLowerCase();
      }

      return value[sortHeaderId];
    };

    this.matDataSource.filterPredicate = (
      data: DocumentoPendenteAssinaturaModel,
      filter: string
    ) => {

      if (
        (data.hasOwnProperty('numero') &&
          data.numero !== null &&
          data.numero
            .toString()
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.descricao !== null &&
          data.descricao
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.titulo !== null &&
          data.titulo
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.nomeDocumento !== null &&
          data.nomeDocumento
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.descricaoCategoria !== null &&
          data.descricaoCategoria
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.dataCriacao !== null &&
          DataHelper.parseToPortugueseStringDate(
            new Date(data.dataCriacao)
          ).indexOf(filter.toLowerCase().trim()) > -1)
      ) {
        return true;
      }

      return false;
    };
  }

  isGroup(index, item): boolean {
    return item.group;
  }

  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'dataSource' && changes[propName].currentValue) {
        this.dataChange.next(changes[propName].currentValue);
        this.ordenarDataSourcePorDataCriacao()
      }
    }
  }

  verificarSeOCheckBoxPodeSerExibido(processoAssinaturaDocumentoId: number): boolean {
    if (this.featureTogglesService.verificarSeAFeatureEstaAtiva(FeatureToggles.selecionarApenasArquivosPendentesDeAssinaturaJaVisualizados)) {
      return this.assinaturaService.listarArquivosPendentesDeAssinaturaJaVisualizados().includes(processoAssinaturaDocumentoId);
    }
    return true;
  }

  verificarSeAbaPermiteAssinaturaEmLote(): boolean {
    if (this.dataSource.filter(documento => documento.usuarioDaConsultaAssinou).length > 0){
      return false;
    }
    return true;
  }

  verificarSeAbaPermiteVerDetalheCiencia(): boolean {
    if (this.dataSource.filter(documento => documento.cienciaId !== null && documento.cienciaId > 0).length > 0){
      return true;
    }
    return false;
  }

  selecionarDocumentosLista(): void {
    let qtdSelecionados = 0;
    this.dataSource.forEach(element => {
      if (this.documentosSelecionadosLista.indexOf(element.id) > -1) {
        element.selecionado = true;
        qtdSelecionados++;
        this.selecionarParaAssinar(true, element.id);
      }
    });
    if (qtdSelecionados === this.dataSource.length) {
      this.selecionarTodos = true;
    }
  }

  selecionarParaAssinar(selecionado: boolean, processoAssinaturaDocumentoId: number): void {
    if (selecionado) {
      this.documentosSelecionados.push(processoAssinaturaDocumentoId);
    } else {
      const index: number = this.documentosSelecionados.indexOf(processoAssinaturaDocumentoId);
      if (index !== -1) {
        this.documentosSelecionados.splice(index, 1);
      }
      this.documentosSelecionados.splice(processoAssinaturaDocumentoId);
      this.selecionarTodos = false;
    }
    this.eventDocumentosSelecionados.emit(this.documentosSelecionados);
  }

  selecionarTodosParaAssinar(selecionado: MatCheckboxChange): void {

    this.documentosSelecionados = [];

    this.dataSource.forEach(documento => {
      if (this.verificarSeOCheckBoxPodeSerExibido(documento.id)) {
        if (selecionado) {
          if(!documento.assinarCertificadoDigital) {
            this.documentosSelecionados.push(documento.id);
          }
          this.selecionarTodos = true;
          documento.selecionado = !documento.assinarCertificadoDigital;
        } else {
          this.selecionarTodos = false;
          documento.selecionado = false;
        }
      }
    });
    this.eventDocumentosSelecionados.emit(this.documentosSelecionados);
  }

  visualizarDocumento(documento: any): void {
    if (!this.verificarSeAbaPermiteAssinaturaEmLote()) {
      this.abrirDocumento(documento);
      return;
    }

    if(documento.assinarCertificadoDigital) {
      this.router.navigate(['assinatura/assinar-rejeitar-acao-digital/' + documento.id]);
    }
    else {
      this.router.navigate(['assinatura/assinar-rejeitar-acao/' + documento.id]);
    }
  }

  visualizarCiencia(documento: any): void {

    switch(documento.categoriaId)
    {
      case  CategoriaDocumento.AutorizacaodeSaidadeMaterialCOMRetorno.valueOf():
      case  CategoriaDocumento.AutorizacaodeSaidadeMaterialSEMRetorno.valueOf():
        this.AssinaturaSaidaMaterial(documento,true,this.tituloCiencia + documento.descricaoCategoria ,StatusAcaoMaterial.RegistroSaida);
      break;

      case  CategoriaDocumento.SaidadeMaterialComNotaFiscal.valueOf():
        this.BuscaCienciaMaterialComNf(documento)
        break;

      case CategoriaDocumento.Statuspagamento.valueOf():
        this.BuscaCienciaStatusPagamento(documento);
      break;
    }

  }

  private BuscaCienciaStatusPagamento(statusPagamento: any) {
    this.fi1548Service.obterCienciaPorId(statusPagamento.cienciaId)
      .subscribe((statusPagamentoCiencia) => {
        this.ModalStatusPagamento(statusPagamentoCiencia,this.tituloCiencia + statusPagamento.descricaoCategoria, "Cancelamento");
      });
  }

  ModalStatusPagamento(solicitacaoCiencia: DocumentoFI1548CienciaModel , modalTitulo: string, acao: string) {
    this.PararContador.emit(true);
    const dialogo =  this.dialog.open(AssinaturaCienciaStatusPagamentoAcaoComponent, {
      width: '920px',
      height: '860px',
      disableClose: true,
      data: {
        titulo:modalTitulo,
        ciencia:solicitacaoCiencia,
        tipoAcao:acao,
      }
    });

    dialogo.afterClosed().subscribe(result => {
      if(result !== undefined){
        this.fi1548Service.registroCienciaStatusPagamento(result)
          .subscribe((result) => {
            if(result != null){
              this.AtualizarTabela.emit();
            }
         });
      }
      this.PararContador.emit(false);
  });
}

  private BuscaCienciaMaterialComNf(cienciaMaterial: any) {
    this.serviceMaterialNF.obterCienciaPorId(cienciaMaterial.cienciaId)
      .subscribe((solicitacaoCiencia) => {
        this.ModalSaidaMaterialComNotaFiscal(solicitacaoCiencia,this.tituloCiencia + cienciaMaterial.descricaoCategoria, StatusAcaoMaterial.RegistroRetorno);
      });
  }

  ModalSaidaMaterialComNotaFiscal(solicitacaoCiencia: SaidaMaterialNotaFiscalCienciaModel, modalTitulo: string, acao: StatusAcaoMaterial) {
      this.PararContador.emit(true);
      const dialogo =  this.dialog.open(AssinaturaCienciaMaterialNotaFiscalAcaoComponent, {
        width: '920px',
        height: '860px',
        disableClose: true,
        data: {
          titulo:modalTitulo,
          ciencia:solicitacaoCiencia,
          tipoAcao:acao,
        }
      });

      dialogo.afterClosed().subscribe(result => {
        if(result !== undefined){
          this.serviceMaterialNF.registroCienciaNf(result)
          .subscribe((result) => {
            if(result != null){
              this.AtualizarTabela.emit();
            }
         });
        }
        this.PararContador.emit(false);
    })
  }

  private AssinaturaSaidaMaterial(cienciaMaterial: SolicitacaoCienciaModel,comRetorno:boolean, titulo:string, statusAcaoMaterial:StatusAcaoMaterial) {
    this.fi1347Service.obterCienciaPorId(cienciaMaterial.cienciaId)
      .subscribe((solicitacaoCiencia) => {
        this.ModalSaidaMaterial(solicitacaoCiencia,titulo, StatusAcaoMaterial.RegistroRetorno);
      });
  }

    ModalSaidaMaterial(solicitacaoCiencia: CienciaEhHistoricoResponseModel , modalTitulo: string, acao: StatusAcaoMaterial)
    {
      this.PararContador.emit(true);
      const dialogo =  this.dialog.open(AssinaturaCienciaAcaoComponent, {
        width: '920px',
        height: '860px',
        disableClose: true,
        data: {
          titulo:modalTitulo,
          ciencia:solicitacaoCiencia,
          tipoAcao:acao,
        }
      });

      dialogo.afterClosed().subscribe(result => {
        if(result !== undefined){
          this.fi1347Service.registroCiencia(result)
          .subscribe((result) => {
            if(result != null){
              this.AtualizarTabela.emit();
            }
         });
        }
        this.PararContador.emit(false);
    })

    }




  assinarDocumentosSelecionados(): void {
    if (this.documentosSelecionados.length === 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'É necessário ter ao menos um documento selecionado.');
      return;
    }

    this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
      `Tem certeza que deseja aprovar o(s) documento(s) selecionado(s) ?`)
      .then(resposta => {
        if (resposta.value) {
          this.assinar();
        }
      });
  }

  assinar() {
    this.assinaturaService.assinarRejeitarValidarExisteAssinaturaDocJaAssinado(this.documentosSelecionados).subscribe(
      () => {
        this.assinaturaService.assinar(new AssinaturaPassoItemAssinarRejeitarModel(this.documentosSelecionados, null, null)).subscribe(() => {
            this.documentosSelecionados.forEach(processoAssinaturaDocumentoId => {
              this.assinaturaService.removerArquivoPendenteDeAssinaturaJaVisualizado(processoAssinaturaDocumentoId);
            });
          this.atualizarParent();
        });
      },
      () => {
        this.atualizarParent();
      }
    );
  }

  atualizarParent(): void {
    this.eventAtualizarMatriz.next();
  }

  ordenarDataSourcePorDataCriacao() {
    this.ordenacaoCustomizada(this.sort)
  }

  ordenacaoCustomizada(sort: Sort) {
    if(!this.matDataSource) {
      return;
    }

    this.sort = sort

    const data = this.matDataSource.data;

    if (!sort.active || sort.direction === '') {
      this.matDataSource.data = data;
      return;
    }

    this.matDataSource.data = [...data].sort((itemA, itemB) => {
      const ehAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'numero':
          return this.comparacaoNumeroComDestaqueEmPrioridade(itemA, itemB, ehAsc);
        case 'descricao':
          return this.comparacaoPadrao(itemA.descricao, itemB.descricao, ehAsc);
        case 'descricaoCategoria':
          return this.comparacaoPadrao(itemA.descricaoCategoria, itemB.descricaoCategoria, ehAsc);
        case 'dataCriacao':
          return this.comparacaoPadrao(itemA.dataCriacao, itemB.dataCriacao, ehAsc);
        default:
          return 0;
      }
    });
  }

  comparacaoNumeroComDestaqueEmPrioridade(documentoA: DocumentoPendenteAssinaturaModel, documentoB: DocumentoPendenteAssinaturaModel, ehAsc: boolean) {
    if (!documentoA.destaque && documentoB.destaque) {
      return 1;
    }

    if (documentoA.destaque && !documentoB.destaque) {
      return -1;
    }

    return (documentoA.numero < documentoB.numero ? -1 : 1) * (ehAsc ? 1 : -1);
  }

  comparacaoPadrao(itemA: number | string, itemB: number | string, isAsc: boolean) {
    return (itemA < itemB ? -1 : 1) * (isAsc ? 1 : -1);
  }

  retornarDescricaoStatusPassoAtual(status: AssinaturaStatusPassoAtual) {
    return AssinaturaStatusPassoAtualLabel.get(status);
  }

  retornaClassStatusPasso(statusPasso: string): string {

    if(statusPasso === this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.Assinado)){
      return "marcador-aprovado";
    }

    if(statusPasso ===  this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.AssinadoComObservacao)){
      return "marcador-assinadocomobservacao";
    }

    if(statusPasso ===  this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.Rejeitado)){
      return "marcador-rejeitado";
    }

    return "marcador-naoassinado";
  }

  paddingParaCiencia(row: any):string{
    if(row.observacao != null && row.observacao != undefined && row.observacao != "" )
    return "forcePadding"

    return ""
  }

  retornaDescricaoStatusPasso(statusPasso: string): string {

    if(statusPasso === this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.Assinado)){
      return "Aprovado";
    }

    if(statusPasso ===  this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.AssinadoComObservacao)){
      return "Aprovado com observação";
    }

    if(statusPasso ===  this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.Rejeitado)){
      return "Rejeitado";
    }

    return "Não aprovado";
  }

  obterProcessoComPassosParaAssinatura(documento: DocumentoPendenteAssinaturaModel) {
    this.assinaturaService.obterProcessoComPassosParaAssinatura(documento.id)
      .subscribe(processo => {
        this.dialog.open(AssinaturaGerenciamentoPendenciaAssinaturaComponent, {
          disableClose: true,
          height: '768',
          width: '800px',
          data: processo
        });
      });
  }

  abrirDocumento(documento: DocumentoPendenteAssinaturaModel) {
    this.assinaturaService.listarArquivosUploadPorPadId(documento.id)
      .subscribe(arquivos => {
        this.dialog.open(PdfPreviewForDialogComponent, {
          disableClose: true,
          width: '920px',
          data: new PdfPreviewForDialogModel(arquivos[arquivos.length-1].arquivoBinario, `Documento_${documento.numero}`, 'PDF - Detalhes do documento')
        });
      });
  }
}
