import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatSortable } from '@angular/material';
import { BehaviorSubject } from 'rxjs';
import { AssinaturaService } from 'src/app/assinatura/assinatura.service';
import { AssinaturaAssinanteRepresentanteModel } from 'src/app/assinatura/models/assinatura-assinante-representante.model';
import { MatTableDataSource } from 'src/app/shared/mat-table-groupby/table-data-source';

@Component({
  selector: 'app-assinatura-gerenciamento-representante-tabela',
  templateUrl: './representante-tabela.component.html',
  styleUrls: ['./representante-tabela.component.scss']
})
export class AssinaturaGerenciamentoRepresentanteTabelaComponent implements OnInit, OnChanges {
  @Input() dataSource: AssinaturaAssinanteRepresentanteModel[] = [];
  @Output() dataSourceChange = new EventEmitter<AssinaturaAssinanteRepresentanteModel[]>();

  displayedColumns: string[] = [
    'usuarioAdAssinate',
    'usuarioAdRepresentante'
  ];

  matDataSource: MatTableDataSource<AssinaturaAssinanteRepresentanteModel>;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataChange = new BehaviorSubject<AssinaturaAssinanteRepresentanteModel[]>([]);

  constructor(
    private readonly assinaturaService: AssinaturaService
  ) { }

  ngOnInit() {
    this.configurarDataSource();
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

    this.matDataSource = new MatTableDataSource<AssinaturaAssinanteRepresentanteModel>();
    this.matDataSource.sort = this.sort;

    this.dataChange.subscribe(data => {
      this.matDataSource.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.matDataSource.paginator = this.paginator;
    });

    this.matDataSource.sortingDataAccessor = (value: any, sortHeaderId: string): string => {
      if (sortHeaderId === 'data' && value[sortHeaderId] !== undefined) {
        return value[sortHeaderId].toLocaleLowerCase();
      }

      if (typeof value[sortHeaderId] === 'string') {
        return value[sortHeaderId].toLocaleLowerCase();
      }

      return value[sortHeaderId];
    };

    this.matDataSource.filterPredicate = (data: AssinaturaAssinanteRepresentanteModel, filter: string) => {
      if (
            (
                data.usuarioAdAssinate !== null
                && data.usuarioAdAssinate.toLowerCase().trim().indexOf(filter.toLowerCase().trim()) > -1
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
      id: 'usuarioAdAssinate',
      start: 'asc'
    } as MatSortable);
  }

  assinaturaAssinanteRepresentanteOnChange(event: AssinaturaAssinanteRepresentanteModel){
    const index = this.dataSource.findIndex(obj => obj.passoId === event.passoId);
    this.dataSource[index].usuarioAdRepresentanteGuid = event.usuarioAdRepresentanteGuid;
    this.dataSource[index].usuarioAdRepresentante = event.usuarioAdRepresentante;
    this.dataSourceChange.emit([...this.dataSource]);
  }
}
