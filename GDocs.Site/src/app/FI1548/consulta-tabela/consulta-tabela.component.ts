import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort, MatSortable } from '@angular/material';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { AutenticacaoService } from 'src/app/autenticacao/services/autenticacao.service';
import { ExibicaoDeAlertaService } from 'src/app/core/services/exibicao-de-alerta/exibicao-de-alerta.service';
import { TipoAlertaEnum } from 'src/app/core/services/exibicao-de-alerta/tipo-alerta.enum';
import { DataHelper } from 'src/app/helpers';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { Grouping, MatGroupBy } from 'src/app/shared/mat-table-groupby/groupBy';
import { MatTableDataSource } from 'src/app/shared/mat-table-groupby/table-data-source';
import { Fi1548Service } from '../FI1548.service';
import { PdfComponent } from '../pdf/pdf.component';
import { DocumentoModel } from '../shared';
import { StatusDocumento } from '../shared/enums/status-documento';

@Component({
  selector: 'app-consulta-tabela',
  templateUrl: './consulta-tabela.component.html',
  styleUrls: ['./consulta-tabela.component.scss'],
})
export class ConsultaTabelaComponent implements OnInit, OnChanges {
  @Input() dataSource: DocumentoModel[] = [];

  displayedColumns: string[] = [
    'numero',
    'referencia',
    'tipoPagamento',
    'valor',
    'status',
    'dataCriacao',
    'acao',
  ];

  matDataSource: MatTableDataSource<DocumentoModel>;

  @Output() cancelarEventEmitter: EventEmitter<boolean> = new EventEmitter();

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataChange = new BehaviorSubject<DocumentoModel[]>([]);
  tipoPagamentoLabel: Map<string, string>;
  constructor(
    private readonly fi1548Service: Fi1548Service,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly router: Router,
    public dialog: MatDialog,
    public matGroupBy: MatGroupBy<DocumentoModel>,
    public datepipe: DatePipe,
    public currencyPipe: CurrencyPipe
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'dataSource' && changes[propName].currentValue) {
        this.dataChange.next(changes[propName].currentValue);
      }
    }
  }

  @HostListener('matSortChange', ['$event'])

  sortChange(e) {
    localStorage.setItem('ordenacaoStatusPagamento', JSON.stringify(e));
  }

  ngOnInit() {
    this.configurarDataSource();

    this.tipoPagamentoLabel = new Map<string, string>([
      ['Unico', 'Único'],
      ['Parcial', 'Parcial'],
      ['Parcelado', 'Parcelado'],
    ]);
  }

  aplicarFiltro(filterValue: string) {
    this.matDataSource.filter = filterValue.trim().toLowerCase();
  }

  private configurarDataSource(): void {
    this.matDataSource = new MatTableDataSource<DocumentoModel>();
    this.matDataSource.sort = this.sort;

    this.dataChange.subscribe((data) => {
      this.matDataSource.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.matDataSource.paginator = this.paginator;
      this.matGroupBy.grouping = new Grouping();
      this.matGroupBy.grouping.groupingDataAccessor = (row) =>
        `${row.autor.toUpperCase()}`;
      this.matDataSource.groupBy = this.matGroupBy;
    });

    this.matDataSource.sortingDataAccessor = (
      value: any,
      sortHeaderId: string
    ): string => {
      if (sortHeaderId === 'dataCriacao' && value[sortHeaderId] !== undefined) {
          return value[sortHeaderId];
      }
      return value[sortHeaderId];

    };

    this.matDataSource.filterPredicate = (
      data: DocumentoModel,
      filter: string
    ) => {
      if (
        (data.autor !== null &&
          data.autor.toLowerCase().trim().indexOf(filter.toLowerCase().trim()) >
          -1) ||
        (data.id !== null &&
          data.id
            .toString()
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.numero !== null &&
          data.numero
            .toString()
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.referencia !== null &&
          data.referencia
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.descricao !== null &&
          data.descricao
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.tipoPagamento !== null &&
          data.tipoPagamento
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.valor !== null &&
          this.currencyPipe.transform(data.valor, 'BRL')
            .toString()
            .toLowerCase()
            .trim()
            .replace(/\s/g, '')
            .indexOf(filter.replace(/\s/g, '').toLowerCase().trim()) > -1) ||
        (data.valor !== null &&
          data.valor
            .toString()
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.status !== null &&
          data.status
            .toString()
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

    this.ordenarDataSourcePorDataCriacao();

  }

  ordenarDataSourcePorDataCriacao() {
    this.sort.sort({
      start: "desc",
      id: "dataCriacao"
    } as MatSortable);
  }

  converterDataStringParaCompare(data: string): string {
    return this.datepipe.transform(new Date(data), 'yyyy-MM-dd');
  }

  isGroup(index, item): boolean {
    return item.group;
  }

  confirmarCancelamento(documento: DocumentoModel): void {
    if (documento.status.toString() === StatusDocumento[StatusDocumento.EmAberto])
      this.cancelamentoComNecessidadeMotivo(documento);
    else
      this.cancelamentoSemNecessidadeMotivo(documento);
  }

  cancelamentoSemNecessidadeMotivo(documento: DocumentoModel): void {

    this.exibicaoDeAlertaService
      .exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja cancelar o documento ${documento.numero}?`)
      .then((resposta) => {
        if (resposta.value) {
          this.executarCancelamento(documento, '');
        }
      });

  }

  cancelamentoComNecessidadeMotivo(documento: DocumentoModel): void {

    const dialogRef = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '285px',
      disableClose: true,
      data: {
        titulo: 'Motivo de cancelamento',
        confirmar: 'Confirmar',
        nomeCampo:'Motivo',
        maxLength: 200
      }
    });

    dialogRef.afterClosed().subscribe(retorno => {
      if (retorno === undefined || retorno.justificativa === null) {
        return;
      }
      this.exibicaoDeAlertaService
      .exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja cancelar o documento ${documento.numero}?`)
      .then((resposta) => {
        if (resposta.value) {
          this.executarCancelamento(documento, retorno.justificativa);
        }
      });
    });

  }

  executarCancelamento(documento: DocumentoModel, motivo: string) {

    this.fi1548Service
      .cancelarDocumento(documento.id, motivo)
      .subscribe((documentoCancelado) => {
        if (documentoCancelado !== null && documentoCancelado !== undefined) {
          this.alertSucessoCancelamento(documentoCancelado);
        }
      });
  }

  liquidarDocumento(documento: DocumentoModel) {
    this.router.navigate(['/fi1548/liquidar/' + documento.numero]);
  }

  obterDescricaoTipoPagamento(documento: DocumentoModel): string {
    return this.tipoPagamentoLabel.get(documento.tipoPagamento);
  }

  mostrarBotaoCancelar(documento: DocumentoModel) {

    return (
      (
        StatusDocumento[documento.status.toString()] === StatusDocumento.EmConstrucao ||
        StatusDocumento[documento.status.toString()] === StatusDocumento.EmAberto ||
        StatusDocumento[documento.status.toString()] === StatusDocumento.PendenteAprovacao
      )
      &&
      (
        this.autenticacaoService.validarPermissao('fi1548:consultar:todosusuarios:cancelartodos') ||
        this.autenticacaoService.obterUsuarioLogado().id === documento.autorId
      )
    );
  }

  mostrarBotaoLiquidar(documento: DocumentoModel) {
    return (
      StatusDocumento[documento.status.toString()] ===
      StatusDocumento.EmAberto &&
      this.autenticacaoService.validarPermissao('fi1548:consultar:liquidar')
    );
  }

  mostrarQuantidadeParcelas(documento: DocumentoModel) {
    return documento.tipoPagamento === 'Parcelado';
  }

  exibirDetalhesDocumento(documento: DocumentoModel): void {
    this.fi1548Service.obterPdf(documento.id).subscribe((pdfDoc) => {

      if(pdfDoc != null){
        documento.binario = pdfDoc.pdfDocumentBase64;
        this.dialog.open(PdfComponent, {
          width: '920px',
          data: documento,
          disableClose: true,
        });
      }
    });
  }

  private alertSucessoCancelamento(documento: DocumentoModel) {
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: documento.status.toString() === StatusDocumento[StatusDocumento.Cancelado] ? `Documento ${documento.numero} foi cancelado com sucesso!` : 'A solicitação de cancelamento foi para análise de ciência.',
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    }).then(() => {
      this.cancelarEventEmitter.emit(true);
    });
  }
}
