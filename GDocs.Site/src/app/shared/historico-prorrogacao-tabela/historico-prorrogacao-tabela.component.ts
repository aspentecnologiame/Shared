import { Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { BehaviorSubject } from 'rxjs';
import { StatusAcaoMaterial } from 'src/app/FI347/shared/enums/status-acao-materia';
import { HistorioProrrogacaoModel } from 'src/app/FI347/shared/models/historico-prorrogacao.model';

@Component({
  selector: 'app-historico-prorrogacao-tabela',
  templateUrl: './historico-prorrogacao-tabela.component.html'
})
export class HistoricoProrrogacaoTabelaComponent  implements OnInit {

  descricaoColunaPara:string = "Para";
  displayedColumns = ['de','para','observacao','status'];
  dataChange = new BehaviorSubject<HistorioProrrogacaoModel[]>([]);
  dataSourceHistorico: MatTableDataSource<HistorioProrrogacaoModel> = new MatTableDataSource<HistorioProrrogacaoModel>();
  mostraTitulo = true;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;


  @Input() dataSource: HistorioProrrogacaoModel[] = [];

  @Input()
  set MostraTitulo(acao:boolean) {
      this.mostraTitulo = acao;
  }


  @Input()
  set configuracao(parametro: number) {
      if(parametro ==  StatusAcaoMaterial.BaixaMaterialSemRetorno.valueOf()){
        this.displayedColumns = ['para','observacao','status'];
        this.descricaoColunaPara = "Data";

      }

  }

  MostrarInfo(parametro:any):boolean{
    return (parametro != null && parametro != undefined && parametro != "")
  }

  VerificaMostraTitulo():boolean{
    return this.mostraTitulo
  }

  ngOnInit(): void {
    this.ConfigurarTabela();
  }

  private ConfigurarTabela(): void {
    this.dataSourceHistorico = new MatTableDataSource<HistorioProrrogacaoModel>();
    this.dataSourceHistorico.sort = this.sort;

    this.dataChange.subscribe((data) => {
      this.dataSourceHistorico.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [3,10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.dataSourceHistorico.paginator = this.paginator;
    });

    this.dataSourceHistorico.data = this.dataSource;
  }


  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'dataSource' && changes[propName].currentValue) {
        this.dataChange.next(changes[propName].currentValue);
      }
    }
  }
}
