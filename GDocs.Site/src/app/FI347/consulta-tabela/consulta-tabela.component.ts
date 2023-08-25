import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort, MatSortable } from '@angular/material';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { AutenticacaoService } from '../../autenticacao/services/autenticacao.service';
import { ExibicaoDeAlertaService, TipoAlertaEnum } from '../../core/services/exibicao-de-alerta';
import { Fi1347Service } from '../../FI347/FI347.service';
import { PdfComponent } from '../../FI347/pdf/pdf.component';
import { StatusSaidaMaterial } from '../../FI347/shared/enums/status-saida-material';
import { SaidaMaterialModel } from '../../FI347/shared/models/saida-material.model';
import { DataHelper } from '../../helpers/data-helper';
import { Grouping, MatGroupBy } from '../../shared/mat-table-groupby/groupBy';
import { MatTableDataSource } from '../../shared/mat-table-groupby/table-data-source';
import { HistoricoMaterialComponent } from '../historico-material/historico-material.component';
import { MaterialSaidaAcaoComponent } from '../material-saida-acao/material-saida-acao.component';
import { StatusAcaoMaterial } from '../shared/enums/status-acao-materia';

@Component({
  selector: 'app-consulta-tabela',
  templateUrl: './consulta-tabela.component.html',
  styleUrls: ['./consulta-tabela.component.scss']
})
export class ConsultaTabelaComponent implements OnInit {

  @Input() dataSource: SaidaMaterialModel[] = [];

  displayedColumns: string[] = [
    'numero',
    'tipoSaida',
    'destino',
    'status',
    'dataAcaoSaida',
    'retorno',
    'acao',
  ];

  matDataSourceTable: MatTableDataSource<SaidaMaterialModel>;

  @Output() cancelarEventEmitter: EventEmitter<boolean> = new EventEmitter();

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataChange = new BehaviorSubject<SaidaMaterialModel[]>([]);
  resultPdfView:any;
  constructor(
    private readonly fi1347Service: Fi1347Service,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly router: Router,
    public dialog: MatDialog,
    public matGroupBy: MatGroupBy<SaidaMaterialModel>,
    public datepipe: DatePipe,
    public currencyPipe: CurrencyPipe
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    for (const propriedade in changes) {
      if (propriedade === 'dataSource' && changes[propriedade].currentValue) {
        this.dataChange.next(changes[propriedade].currentValue);
      }
    }
  }

  @HostListener('matSortChange', ['$event'])

  sortChange(e) {
    localStorage.setItem('ordenacaoSaidaMaterial', JSON.stringify(e));
  }

  ngOnInit() {
    this.configurarDataSource();
  }

  aplicarFiltro(filterValue: string) {
    this.matDataSourceTable.filter = filterValue.trim().toLowerCase();
  }

  private configurarDataSource(): void {
    this.matDataSourceTable = new MatTableDataSource<SaidaMaterialModel>();
    this.matDataSourceTable.sort = this.sort;

    this.dataChange.subscribe((data) => {
      this.matDataSourceTable.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.matDataSourceTable.paginator = this.paginator;
      this.matGroupBy.grouping = new Grouping();
      this.matGroupBy.grouping.groupingDataAccessor = (row) =>
        `${row.nomeResponsavel.toUpperCase()}`;
      this.matDataSourceTable.groupBy = this.matGroupBy;
    });

    this.matDataSourceTable.sortingDataAccessor = (
      value: any,
      sortHeaderId: string
    ): string => {

      if (sortHeaderId === 'retorno' && value[sortHeaderId] !== undefined) {
        return  DataHelper.addSpecificTimeToDateTime(new Date(value[sortHeaderId]), 12, 0).getTime().toString() + value["autor"];
      }

      if (sortHeaderId === 'dataAcaoSaida' && value["dataAcao"] !== undefined) {
        return  DataHelper.addSpecificTimeToDateTime(new Date(value["dataAcao"]), 12, 0).getTime().toString() + value["autor"];
      }

      if (typeof value[sortHeaderId] === 'string') {
        return value[sortHeaderId].toLocaleLowerCase() + value["autor"];
      }

      return value[sortHeaderId];
    };

    this.matDataSourceTable.filterPredicate = (
      data: SaidaMaterialModel,
      filter: string
    ) => {
      if (
          this.filtrarCampoTexto(data, 'destino', filter) ||
          this.filtrarCampoTexto(data, 'htmlItens', filter) ||
          this.filtrarCampoTexto(data, 'tipoSaida', filter) ||
          this.filtrarCampoTexto(data, 'guidResponsavel', filter) ||
          this.filtrarCampoTexto(data, 'status', filter) ||
        (data.numero !== null &&
          data.numero
            .toString()
            .toLowerCase()
            .trim()
            .indexOf(filter.toLowerCase().trim()) > -1) ||
        (data.retorno !== null &&
          DataHelper.parseToPortugueseStringDate(
            new Date(data.retorno)
          ).indexOf(filter.toLowerCase().trim()) > -1)
          ||
        (data.dataAcao !== null &&
          DataHelper.parseToPortugueseStringDate(
            new Date(data.dataAcao)
          ).indexOf(filter.toLowerCase().trim()) > -1)
      )

      {
        return true;
      }
      return false;
    };

    this.ordenarDataSourcePadrao();

  }

  ordenarDataSourcePadrao() {
    this.sort.sort({
      start: "desc",
      id: "retorno"
    } as MatSortable);
  }

  converterDataStringParaCompare(data: string): string {
    return this.datepipe.transform(new Date(data), 'yyyy-MM-dd');
  }

  isGroup(index, item): boolean {
    return item.group;
  }

  confirmarCancelamento(saidaMaterial: SaidaMaterialModel): void {
    if (saidaMaterial.statusId == StatusSaidaMaterial.PendenteSaida)
      this.cancelamentoComNecessidadeMotivo(saidaMaterial);
    else
      this.cancelamentoSemNecessidadeMotivo(saidaMaterial);
  }

  cancelamentoSemNecessidadeMotivo(saidaMaterial: SaidaMaterialModel): void {
    this.exibicaoDeAlertaService
      .exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja cancelar a solicitação de saída de material ${saidaMaterial.numero}?`)
      .then((resposta) => {
        if (resposta.value) {
          this.executarCancelamento(saidaMaterial, '');
        }
      });
  }

  cancelamentoComNecessidadeMotivo(saidaMaterial: SaidaMaterialModel): void {

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
      .exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja cancelar a solicitação de saída de material ${saidaMaterial.numero}?`)
      .then((resposta) => {
        if (resposta.value) {
          this.executarCancelamento(saidaMaterial, retorno.justificativa);
        }
      });
    });

  }

  executarCancelamento(saidaMaterial: SaidaMaterialModel, motivo: string): void {
      this.fi1347Service
      .cancelarSaidaMaterial(saidaMaterial.id, motivo)
      .subscribe((saidaMaterialCancelado) => {
        if (saidaMaterialCancelado !== null && saidaMaterialCancelado !== undefined) {
          this.alertSucessoCancelamento(saidaMaterialCancelado);
        }
      });
  }

  mostrarBotaoCancelar(saidaMaterial: SaidaMaterialModel) {
    return (
      (
        saidaMaterial.statusId === StatusSaidaMaterial.EmConstrucao.valueOf() ||
        saidaMaterial.statusId === StatusSaidaMaterial.PendenteAprovacao.valueOf() ||
        saidaMaterial.statusId === StatusSaidaMaterial.PendenteSaida.valueOf()
      )
      &&
      (
        this.autenticacaoService.validarPermissao('fi347:consultar:todosusuarios:cancelartodos') ||
        this.autenticacaoService.obterUsuarioLogado().id === saidaMaterial.guidResponsavel
      )
    );
  }

  mostraBotaoHistoricoMaterial():boolean{
    return this.autenticacaoService.validarPermissao('fi347:consultar');
  }

  mostraBotaoRegistroSaida(saidaMaterial: SaidaMaterialModel){
    return (
       this.autenticacaoService.validarPermissao('fi347:acoes:saida') &&
       saidaMaterial.statusId === StatusSaidaMaterial.PendenteSaida.valueOf()
       );
  }

  mostraBotaoRegistroRetorno(saidaMaterial: SaidaMaterialModel){
    return (
      this.autenticacaoService.validarPermissao('fi347:acoes:retorno') &&
      saidaMaterial.statusId === StatusSaidaMaterial.EmAberto.valueOf()
      );
  }

  mostraBotaoRegistroProrrogacao(saidaMaterial: SaidaMaterialModel){
    return (
      saidaMaterial.statusId === StatusSaidaMaterial.EmAberto.valueOf() &&
      saidaMaterial.autor == this.autenticacaoService.obterUsuarioLogado().id
      );
  }

  mostraBotaoRegistroBaixaSemRetorno(saidaMaterial: SaidaMaterialModel){
    return (
      saidaMaterial.statusId === StatusSaidaMaterial.EmAberto.valueOf() &&
      (
      this.autenticacaoService.validarPermissao('fi347:acoes:baixasemretorno') ||
      saidaMaterial.autor == this.autenticacaoService.obterUsuarioLogado().id ||
      saidaMaterial.guidResponsavel == this.autenticacaoService.obterUsuarioLogado().id
      )
      );
  }

  exibirDetalhesDocumento(saidaMaterial: SaidaMaterialModel): void {
    this.fi1347Service.obterPdf(saidaMaterial.id).subscribe((pdfDoc) => {
      if (pdfDoc != null) {
        saidaMaterial.binario = pdfDoc.pdfDocumentBase64;
        this.dialog.open(PdfComponent, {
          width: '920px',
          data: saidaMaterial,
          disableClose: true,
        });
      }
    });
  }

    HistoricoMaterial(saidaMaterial: SaidaMaterialModel): void {
      this.fi1347Service.obterHistoricoMaterial(saidaMaterial.id)
      .subscribe((resultHistorico) => {
        this.dialog.open(HistoricoMaterialComponent, {
          width: '920px',
          height: '900px',
          disableClose: true,
          data: {
            historico:resultHistorico,
          }
        });
      });
    }


    FazerRegistroSaidaMaterial(saidaMaterial: SaidaMaterialModel): void {
     this.movimentacaoAcao(saidaMaterial,
      "Registro de Saída"
      ,StatusAcaoMaterial.RegistroSaida);
    }

    FazerRegistroRetornoMaterial(saidaMaterial: SaidaMaterialModel): void {
     this.movimentacaoAcao(saidaMaterial,
      "Registro de Retorno"
      ,StatusAcaoMaterial.RegistroRetorno);
    }


    SolicitacaoProrrogacaoRetorno(saidaMaterial: SaidaMaterialModel): void {
     this.movimentacaoAcao(saidaMaterial,"Solicitação de Prorrogação de retorno"
     ,StatusAcaoMaterial.SolicitacaoProrrogacao);
    }

    BaixaMaterialSemRetorno(saidaMaterial: SaidaMaterialModel): void {
     this.movimentacaoAcao(saidaMaterial,"Solicitação de Baixa de material sem retorno",
     StatusAcaoMaterial.SolicitacaoBaixaMaterialSemRetorno,
     StatusAcaoMaterial.BaixaMaterialSemRetorno,
     );
    }

    private movimentacaoAcao(saidaMaterial: SaidaMaterialModel,titulo:string, statusAcaoMaterial:StatusAcaoMaterial, buscaItem?:StatusAcaoMaterial) {
      this.fi1347Service.obterListarItemMaterial(saidaMaterial.id, (buscaItem != null?buscaItem.valueOf():statusAcaoMaterial.valueOf()))
        .subscribe((result) => {
          saidaMaterial.itemMaterial = result;
          this.ModalSaidaMaterial(saidaMaterial,titulo, statusAcaoMaterial);
        });
    }


  ModalSaidaMaterial(saidaMaterial: SaidaMaterialModel , modalTitulo: string, acao: StatusAcaoMaterial)
  {
    const dialogo =  this.dialog.open(MaterialSaidaAcaoComponent, {
      width: '920px',
      height: '830px',
      disableClose: true,
      data: {
        titulo:modalTitulo,
        editarItem:saidaMaterial,
        tipoAcao:acao
      }
    });

    dialogo.afterClosed().subscribe(result => {
      if(result !== undefined){
        this.fi1347Service.registroMovimentacaoMaterial(result)
        .subscribe((result) => {
          if(result != null)
          this.cancelarEventEmitter.emit(true);
        });
      }
  })
}

  private alertSucessoCancelamento(saidaMaterial: SaidaMaterialModel) {
     this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: saidaMaterial.status == 'Cancelado' ? `Saída de material - sem NF ${saidaMaterial.numero} foi cancelada com sucesso!` : 'A solicitação de cancelamento foi para análise de ciência.',
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    }).then(() => {
      this.cancelarEventEmitter.emit(true);
    });
  }

  private filtrarCampoTexto(data: SaidaMaterialModel, nomeCampo: string, filter: string): boolean {
    return (data[nomeCampo] !== null &&
      data[nomeCampo].toLowerCase().trim().indexOf(filter.toLowerCase().trim()) >
      -1);
  }
}
