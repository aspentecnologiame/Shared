import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, EventEmitter, HostListener, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort, MatSortable } from '@angular/material';
import { Router } from '@angular/router';
import { BehaviorSubject } from 'rxjs';
import { JustificativaModalComponent } from 'src/app/shared/justificativa-modal/justificativa-modal.component';
import { AutenticacaoService } from '../../autenticacao/services/autenticacao.service';
import { ExibicaoDeAlertaService, TipoAlertaEnum } from '../../core/services/exibicao-de-alerta';
import { DataHelper } from '../../helpers/data-helper';
import { Grouping, MatGroupBy } from '../../shared/mat-table-groupby/groupBy';
import { MatTableDataSource } from '../../shared/mat-table-groupby/table-data-source';
import { HistoricoNotaFiscalComponent } from '../historico-nota-fiscal/historico-nota-fiscal.component';
import { PdfComponent } from '../pdf/pdf.component';
import { SaidaMaterialNotaFiscalAcaoComponent } from '../saida-material-nota-fiscal-acao/saida-material-nota-fiscal-acao.component';
import { SaidaMaterialNotaFiscalService } from '../saida-material-nota-fiscal.service';
import { StatusAcaoMaterialNotaFiscal } from '../shared/enums/StatusAcaoMaterialNotaFiscal';
import { StatusSaidaMaterialNF } from '../shared/enums/StatusSaidaMaterialNF';
import { SaidaMaterialNotaFiscal } from './../shared/models/SaidaMaterialNotaFiscal';

@Component({
  selector: 'app-consulta-tabela',
  templateUrl: './consulta-tabela.component.html'
})
export class ConsultaTabelaComponent implements OnInit {

  @Input() dataSource: SaidaMaterialNotaFiscal[] = [];

  displayedColumns: string[] = [
    'numeroNf',
    'tipoSaidaNf',
    'destinoNf',
    'statusNf',
    'dataAcaoNf',
    'retornoNf',
    'acaoNf',
  ];

  matDataSourceMaterialNF: MatTableDataSource<SaidaMaterialNotaFiscal>;

  @Output() cancelarEventEmitter: EventEmitter<boolean> = new EventEmitter();

  @ViewChild(MatPaginator) paginatorConsultaMaterial: MatPaginator;
  @ViewChild(MatSort) ordenarConsutaMaterial: MatSort;

  dataChange = new BehaviorSubject<SaidaMaterialNotaFiscal[]>([]);

  msgCancelado ='Saída de material com nota fiscal foi cancelada com sucesso!';
  msgPendenteCancelamento ='O cancelamento foi encaminhada para contabilidade.';

  constructor(
    private readonly service: SaidaMaterialNotaFiscalService,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly router: Router,
    public dialog: MatDialog,
    public matGroupBy: MatGroupBy<SaidaMaterialNotaFiscal>,
    public datepipe: DatePipe,
    public currencyPipe: CurrencyPipe
  ) { }

  ngOnChanges(simplesMudancas: SimpleChanges): void {
    for (const mudanca in simplesMudancas) {
      if (mudanca === 'dataSource' && simplesMudancas[mudanca].currentValue) {
        this.dataChange.next(simplesMudancas[mudanca].currentValue);
      }
    }
  }

  @HostListener('matSortChange', ['$event'])

  sortChange(e) {
    localStorage.setItem('ordenacaoSaidaMaterialNF', JSON.stringify(e));
  }

  ngOnInit() {
    this.configurarDataSourceConsulta();
  }

  aplicarFiltro(filterValue: string) {
    this.matDataSourceMaterialNF.filter = filterValue.trim().toLowerCase();
  }

  private configurarDataSourceConsulta(): void {
    this.matDataSourceMaterialNF = new MatTableDataSource<SaidaMaterialNotaFiscal>();
    this.matDataSourceMaterialNF.sort = this.ordenarConsutaMaterial;

    this.dataChange.subscribe((data) => {
      this.matDataSourceMaterialNF.data = data;
      this.paginatorConsultaMaterial.length = data.length;
      this.paginatorConsultaMaterial.pageSizeOptions = [10, 20, 30, 40, 50, 100];
      this.paginatorConsultaMaterial.pageIndex = 0;
      this.matDataSourceMaterialNF.paginator = this.paginatorConsultaMaterial;
      this.matGroupBy.grouping = new Grouping();
      this.matGroupBy.grouping.groupingDataAccessor = (row) =>
        `${row.nomeAutor.toUpperCase()}`;
      this.matDataSourceMaterialNF.groupBy = this.matGroupBy;
    });

    this.matDataSourceMaterialNF.sortingDataAccessor = (
      value: any,
      sortHeaderId: string
    ): string => {

      if (sortHeaderId === 'retorno' && value[sortHeaderId] !== undefined) {
        return DataHelper.addSpecificTimeToDateTime(new Date(value[sortHeaderId]), 12, 0).getTime().toString() + value["guidAutor"];
      }

      if (typeof value[sortHeaderId.replace('Nf', '')] === 'string') {
        return value[sortHeaderId.replace('Nf', '')].toLocaleLowerCase() + value["guidAutor"];
      }

      return value[sortHeaderId.replace('Nf', '')];
    };

    this.matDataSourceMaterialNF.filterPredicate = (
      data: SaidaMaterialNotaFiscal,
      filter: string
    ) => {
      if (
          this.filtrarTexto(data, 'destino', filter) ||
          this.filtrarTexto(data, 'htmlItens', filter) ||
          this.filtrarTexto(data, 'tipoSaida', filter) ||
          this.filtrarTexto(data, 'guidAutor', filter) ||
          this.filtrarTexto(data, 'status', filter) ||
        (data.numero !== null && data.numero !== undefined  &&
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
      ) {
        return true;
      }
      return false;
    };

    this.ordenarPadrao();

  }

  ordenarPadrao() {
    this.ordenarConsutaMaterial.sort({
      start: "desc",
      id: "retorno"
    } as MatSortable);
  }

  converterDataStringParaComparar(data: string): string {
    return this.datepipe.transform(new Date(data), 'yyyy-MM-dd');
  }




  mostrarBotaoCancelar(saidaMaterial: SaidaMaterialNotaFiscal) {
    return (
      (
        saidaMaterial.statusId === StatusSaidaMaterialNF.PendenteNfSaida.valueOf() ||
        saidaMaterial.statusId === StatusSaidaMaterialNF.SaidaPendente.valueOf()
      )
      &&
      (
        this.autenticacaoService.obterUsuarioLogado().id === saidaMaterial.guidAutor &&
        this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:cancelar')
      )
    );
  }

  confirmarCancelamento(saidaMaterialNf: SaidaMaterialNotaFiscal){
    this.exibicaoDeAlertaService
    .exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja cancelar a solicitação de saída de material ?`)
    .then((resposta) => {
      if (resposta.value) {
        this.justificativaCancelamento(saidaMaterialNf);
      }
    });
  }

  justificativaCancelamento(saidaMaterialNf:SaidaMaterialNotaFiscal){
    const dialogRef = this.dialog.open(JustificativaModalComponent, {
      width: '600px',
      height: '285px',
      disableClose: true,
      data: {
        titulo: 'Motivo do cancelamento',
        confirmar: 'Confirmar',
        nomeCampo:'Motivo',
        maxLength:255
      }
    });

    dialogRef.afterClosed().subscribe(retorno => {
      if (retorno === undefined || retorno.justificativa === null) {
        return;
      }
        this.service.CancelarSaidaMaterialNF(saidaMaterialNf,retorno.justificativa)
        .subscribe( result=> {
          if(result != null)
            this.alertSucessoCancelamento(saidaMaterialNf.numero != null? this.msgPendenteCancelamento:this.msgCancelado);
        })

    });
  }

  EfetivarCancelamento(saidaMaterialNf:SaidaMaterialNotaFiscal){
    this.exibicaoDeAlertaService
    .exibirMensagemInterrogacaoSimNao(`Tem certeza que deseja cancelar a solicitação de saída de material com NF de numero ${saidaMaterialNf.numero}?`)
    .then((resposta) => {
      if (resposta.value) {
        this.service.EfetivarCancelamentoNF(saidaMaterialNf)
        .subscribe(result => {
          if(result != null)
            this.alertSucessoCancelamento(this.msgCancelado);
        })
      }
    });
  }

  mostrarBotaoEfetivarCancelar(saidaMaterial: SaidaMaterialNotaFiscal):boolean{
    return (this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:efetivarCancelamento') &&
            saidaMaterial.statusId === StatusSaidaMaterialNF.PendenteNfCancelamento.valueOf());

  }
  mostraUploadNotaSaida(saidaMaterial: SaidaMaterialNotaFiscal):boolean{
    return (this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:uploadNotaSaida') &&
            saidaMaterial.statusId === StatusSaidaMaterialNF.PendenteNfSaida.valueOf());
  }

  mostraUploadNotaRetorno(saidaMaterial: SaidaMaterialNotaFiscal):boolean{
    return (this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:retorno:uploadNf') &&
            saidaMaterial.statusId === StatusSaidaMaterialNF.PendenteNfRetorno.valueOf());
  }

  mostraBotaoHistoricoMaterial():boolean{
    return this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:historico');
  }

  mostraBotaoRegistroSaida(saidaMaterial: SaidaMaterialNotaFiscal):boolean{
    return (
       this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:saida') &&
       saidaMaterial.statusId === StatusSaidaMaterialNF.SaidaPendente.valueOf()
       );
  }

  mostraTrocarNotaFiscal(saidaMaterial: SaidaMaterialNotaFiscal):boolean{
    return (
      this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:trocarNotaFiscal') &&
      saidaMaterial.statusId === StatusSaidaMaterialNF.SaidaPendente.valueOf()
      );
  }

  mostraBotaoRegistroRetorno(saidaMaterial: SaidaMaterialNotaFiscal){
    return (
      this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:retorno') &&
      saidaMaterial.statusId === StatusSaidaMaterialNF.EmAberto.valueOf()
      );
  }

  mostraBotaoRegistroProrrogacao(saidaMaterial: SaidaMaterialNotaFiscal){
    return (
      saidaMaterial.statusId === StatusSaidaMaterialNF.EmAberto.valueOf() &&
      (
        this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:prorrogacao') &&
        saidaMaterial.guidAutor == this.autenticacaoService.obterUsuarioLogado().id
       )
      );
  }


  mostraVerDetalhe(saidaMaterial: SaidaMaterialNotaFiscal){
    return (
      saidaMaterial.statusId !== StatusSaidaMaterialNF.PendenteNfSaida.valueOf() &&
      (saidaMaterial.numero != null && saidaMaterial.numero != undefined) &&
      this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:verDetalhes')
      );
  }

  mostraBotaoRegistroBaixaSemRetorno(saidaMaterial: SaidaMaterialNotaFiscal){
    return (
      saidaMaterial.statusId === StatusSaidaMaterialNF.EmAberto.valueOf() &&
      (
      this.autenticacaoService.validarPermissao('SaidaMaterialNF:acao:baixasemretorno') &&
      saidaMaterial.guidAutor == this.autenticacaoService.obterUsuarioLogado().id
      )
      );
  }




  exibirDetalhesDocumento(saidaMaterialNf: any): void {
    this.service.ObterPdfNfPorId(saidaMaterialNf.id).subscribe((pdfDoc) => {
      if(pdfDoc != null ){
        this.dialog.open(PdfComponent, {
          width: '920px',
          data: pdfDoc,
          disableClose: true,
        });
      }});
  }

    HistoricoMaterial(saidaMaterial: SaidaMaterialNotaFiscal): void {
      this.service.obterHistoricoMaterialNotaFiscal(saidaMaterial.id)
      .subscribe((resultHistorico) => {
        this.dialog.open(HistoricoNotaFiscalComponent, {
          width: '920px',
          height: '900px',
          disableClose: true,
          data: {
            historico:resultHistorico,
          }
        });
      });
    }

    ImportNotaFiscal(saidaMaterial: SaidaMaterialNotaFiscal){
      this.router.navigate(['/SaidaMaterialNotaFiscal/uploadNotaFiscal/' + saidaMaterial.id]);
    }

    FazerRegistroSaidaMaterialNf(saidaMaterial: SaidaMaterialNotaFiscal): void {
      this.buscaItemEChamaModalAcao(saidaMaterial,"Registro de Saída",StatusAcaoMaterialNotaFiscal.Saida)
    }

    FazerRegistroRetornoMaterial(saidaMaterial: SaidaMaterialNotaFiscal): void {
      this.buscaItemEChamaModalAcao(saidaMaterial,"Registro de Retorno",StatusAcaoMaterialNotaFiscal.Retorno)
    }


    SolicitacaoProrrogacaoRetorno(saidaMaterial: SaidaMaterialNotaFiscal): void {
      this.buscaItemEChamaModalAcao(saidaMaterial,"Solicitação de Prorrogação de retorno",StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf)
    }

    BaixaMaterialSemRetorno(saidaMaterial: SaidaMaterialNotaFiscal): void {
      this.buscaItemEChamaModalAcao(saidaMaterial,
     "Solicitação de Baixa de material sem retorno",
      StatusAcaoMaterialNotaFiscal.SolicitacaoBaixaMaterialNFSemRetorno,
      StatusAcaoMaterialNotaFiscal.BaixaMaterialNFSemRetorno)

    }

    TrocarNotaFiscalSaida(saidaMaterialNf: SaidaMaterialNotaFiscal){
      this.router.navigate(['/SaidaMaterialNotaFiscal/uploadNotaFiscal/' + saidaMaterialNf.id]);
    }


    private buscaItemEChamaModalAcao(saidaMaterial: SaidaMaterialNotaFiscal,titulo:string, statusAcaoMaterial:StatusAcaoMaterialNotaFiscal, buscaItem?:StatusAcaoMaterialNotaFiscal) {
      this.service.obterListarItemMaterialNotaFiscal(saidaMaterial.id, (buscaItem != null?buscaItem.valueOf():statusAcaoMaterial.valueOf()))
        .subscribe((result) => {
          saidaMaterial.itemMaterialNf = result;
          this.ModalSaidaMaterialNfAcao(saidaMaterial,titulo, statusAcaoMaterial);
        });
    }



  ModalSaidaMaterialNfAcao(saidaMaterial: SaidaMaterialNotaFiscal , modalTitulo: string, acao: StatusAcaoMaterialNotaFiscal)
  {
    const dialogo =  this.dialog.open(SaidaMaterialNotaFiscalAcaoComponent, {
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
          this.service.acaoSaidaMaterialNf(result)
          .subscribe((result) => {
            if(result != null)
            this.cancelarEventEmitter.emit(true);
          });
        }
  })
}



  private alertSucessoCancelamento(msg:string) {
     this.exibicaoDeAlertaService.exibirMensagem({
      titulo: 'Sucesso!',
      mensagem: msg,
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: 'OK',
    }).then(() => {
      this.cancelarEventEmitter.emit(true);
    });
  }

  private filtrarTexto(data: SaidaMaterialNotaFiscal, nomeCampo: string, filter: string): boolean {
    return (data[nomeCampo] !== null &&
      data[nomeCampo].toLowerCase().trim().indexOf(filter.toLowerCase().trim()) >
      -1);
  }
}
