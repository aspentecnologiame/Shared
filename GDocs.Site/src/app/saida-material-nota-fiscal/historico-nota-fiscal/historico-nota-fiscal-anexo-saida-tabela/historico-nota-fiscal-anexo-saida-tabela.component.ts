import { Component, Input, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { BehaviorSubject } from 'rxjs';
import { HistoricoTrocaAnexoSaidaModel } from '../../shared/models/Historico-troca-anexo-saida-model';

@Component({
  selector: 'app-historico-nota-fiscal-anexo-saida-tabela',
  templateUrl: './historico-nota-fiscal-anexo-saida-tabela.component.html'
})
export class HistoricoNotaFiscalAnexoSaidaTabelaComponent implements OnInit {

  displayedColumns = ['usuario','motivo','numero','dataAlteracao'];
  dataChange = new BehaviorSubject<HistoricoTrocaAnexoSaidaModel[]>([]);
  dataSourceHisAnexo: MatTableDataSource<HistoricoTrocaAnexoSaidaModel> = new MatTableDataSource<HistoricoTrocaAnexoSaidaModel>();
  mostraTitulo = true;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;


  @Input() dataSource: HistoricoTrocaAnexoSaidaModel[] = [];

  @Input()
  set MostraTitulo(acao:boolean) {
      this.mostraTitulo = acao;
  }

  VerificaMostraTitulo():boolean{
    return this.mostraTitulo
  }

  ngOnInit(): void {
    this.ConfigurarTabela();
  }

  private ConfigurarTabela(): void {
    this.dataSourceHisAnexo = new MatTableDataSource<HistoricoTrocaAnexoSaidaModel>();
    this.dataSourceHisAnexo.sort = this.sort;

    this.dataChange.subscribe((data) => {
      this.dataSourceHisAnexo.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [3,10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.dataSourceHisAnexo.paginator = this.paginator;
    });

    this.dataSourceHisAnexo.data = this.dataSource;
  }


  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'dataSource' && changes[propName].currentValue) {
        this.dataChange.next(changes[propName].currentValue);
      }
    }
  }



}
