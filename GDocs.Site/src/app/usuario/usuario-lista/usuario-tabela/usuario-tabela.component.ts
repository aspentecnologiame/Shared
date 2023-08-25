import { UsuarioModel } from '../../models/usuario.model';
import { UsuarioService } from '../../usuario.service';
import { PerfilModel } from '../../models/perfil.model';
import { BehaviorSubject } from 'rxjs';
import { Component, OnInit, OnChanges, ViewChild, Input, SimpleChanges } from '@angular/core';
import { MatTableDataSource, MatPaginator, MatSort } from '@angular/material';
import { Router } from '@angular/router';

@Component({
  selector: 'app-usuario-tabela',
  templateUrl: './usuario-tabela.component.html',
  styleUrls: ['./usuario-tabela.component.scss']
})
export class UsuarioTabelaComponent implements OnInit, OnChanges {

  @Input() dataSource: UsuarioModel[] = [];

  displayedColumns: string[] = ['nome', 'perfil', 'acao'];
  matDataSource: MatTableDataSource<UsuarioModel>;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  dataChange = new BehaviorSubject<UsuarioModel[]>([]);

  perfis: PerfilModel[];

  constructor(
    private readonly usuarioService: UsuarioService,
    private readonly router: Router
  ) { }

  ngOnInit() {
    this.carregarUsuarios();
  }

  private carregarUsuarios(): void {
    this.usuarioService.listarUsuarios(0, '')
      .subscribe(
        resposta => {
          this.dataSource = resposta;
          this.configurarDataSource(this.dataSource);
        },
        error => {
          console.error('Erro no método carregarUsuarios.', error);
        }
      );
  }

  private ordenarDataSourceConfiguracaoPadrao() {
    this.sort.sort({
      id: 'nome',
      start: 'asc',
      disableClear: false
    });
  }

  private configurarDataSource(aplicativos: UsuarioModel[]): void {

    this.matDataSource = new MatTableDataSource<UsuarioModel>(aplicativos);
    this.paginator.length = this.matDataSource.data.length;
    this.paginator.pageSizeOptions = [7, 10, 20, 30, 40, 50, 100];
    this.matDataSource.paginator = this.paginator;
    this.matDataSource.sort = this.sort;

    this.dataChange.subscribe(data => this.matDataSource.data = data);

    this.matDataSource.sortingDataAccessor = (value: any, sortHeaderId: string): string => {
      if (sortHeaderId.indexOf('.') > 0) {
        const fields = sortHeaderId.split('.');
        fields.forEach(field => {
          value = value[field];
        });

        if (typeof value === 'string') {
          return value.toLocaleLowerCase();
        }

        return value;
      }

      if (typeof value[sortHeaderId] === 'string') {
        return value[sortHeaderId].toLocaleLowerCase();
      }
      return value[sortHeaderId];
    };

    this.ordenarDataSourceConfiguracaoPadrao();
  }

  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'dataSource' && changes[propName].currentValue) {
        this.dataChange.next(changes[propName].currentValue);
      }
    }
  }

  alterarUsuario(usuario: UsuarioModel): void {
    localStorage.setItem('editUser', JSON.stringify(usuario));
    this.router.navigate(['/usuario/alterar/' + usuario.activeDirectoryId]);
  }
}
