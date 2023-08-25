import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { concat, Observable, of, Subject } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { ExibicaoDeAlertaService } from '../../../core';
import { MatTableDataSource } from '../../../shared/mat-table-groupby/table-data-source';
import { AssinaturaService } from '../../assinatura.service';
import { AssinaturaUsuarioAdModel, PassoAssinaturaUsuariosAdAdcionadosModel } from '../../models';
import { AssinaturaAssinadoresComponent } from '../assinatura-assinadores/assinatura-assinadores.component';


@Component({
  selector: 'app-assinatura-assinadores-card',
  templateUrl: './assinatura-assinadores-card.component.html',
  styleUrls: ['./assinatura-assinadores-card.component.scss',
    '../assinatura-assinadores/assinatura-assinadores.component.scss']
})
export class AssinaturaAssinadoresCardComponent implements OnInit {
  public passoUsuariosAdAdicionados: PassoAssinaturaUsuariosAdAdcionadosModel;
  public parentRef: AssinaturaAssinadoresComponent;
  obsListaUsuario$: Observable<AssinaturaUsuarioAdModel[]>;
  listaUsuarioLoading = false;
  listaUsuarioinput$ = new Subject<string>();
  selectUsuarios: AssinaturaUsuarioAdModel;
  habilitarCertificadoDigital = false;
  displayedColumns: string[] = ['nome', 'assinarDigitalmente', 'recebernotificacao', 'acao'];
  matDataSource: MatTableDataSource<AssinaturaUsuarioAdModel> = new MatTableDataSource<AssinaturaUsuarioAdModel>();
  clickButton = false;
  @Output() removerUsuarioAssinaturaDocumentoEvent = new EventEmitter<AssinaturaUsuarioAdModel>();
  @Output() alteradoCertificadoDigitalEvent = new EventEmitter<boolean>();

  constructor(private readonly assinaturaService: AssinaturaService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService) {
  }

  ngOnInit(): void {
    this.carregarUsuariosAd()
  }

  private carregarUsuariosAd() {
    this.obsListaUsuario$ = concat(
      of([]),
      this.listaUsuarioinput$.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => this.listaUsuarioLoading = true),
        switchMap(term => {
          if (term === undefined || term === null || term.trim().length < 3) {
            this.listaUsuarioLoading = false;
            return of([]);
          }

          let usuariosJaAdicionados = []

          if (this.exbibirTabelaAssinadores()) {
            usuariosJaAdicionados = Array.from(new Set(this.passoUsuariosAdAdicionados.usuarios.map(u => u.guid)));
          }

          return this.assinaturaService.listarUsuariosAdParaAssinaturaPorNome(term.trim()).pipe(
            map(
              (usuario: AssinaturaUsuarioAdModel[]) =>
                usuario.filter(u => usuariosJaAdicionados.filter(usuarioAdicionadoguid => usuarioAdicionadoguid === u.guid).length === 0)
            ),
            catchError(() => of([])),
            tap(() => this.listaUsuarioLoading = false)
          );
        })
      )
    );
  }


  atualizarUsuariosEdicao() {
    this.matDataSource.data = [...this.passoUsuariosAdAdicionados.usuarios];
  }

  adicionarAssinador() {
    if (this.selectUsuarios === null || this.selectUsuarios === undefined) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Nenhum usuário selecionado.');
      return;
    }

    if (!this.parentRef.verificarSeAssinadorJaEstaEmOutrosPassos(this.selectUsuarios.guid)) {
      this.selectUsuarios.assinarDigitalmente = false;
      this.selectUsuarios.notificarFinalizacao = false;
      this.selectUsuarios.assinarFisicamente = true;
      this.passoUsuariosAdAdicionados.usuarios.push(this.selectUsuarios);
      this.matDataSource.data = [...this.passoUsuariosAdAdicionados.usuarios];
    }

    this.selectUsuarios = null;
    this.carregarUsuariosAd();
  }

  removerAssinador(index: number) {
    this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
      `Tem certeza que deseja remover o aprovador ${this.passoUsuariosAdAdicionados.usuarios[index].nome}?`)
      .then(resposta => {
        if (resposta.value) {
          this.removerUsuarioAssinaturaDocumentoEvent.emit(this.passoUsuariosAdAdicionados.usuarios[index]);
          this.passoUsuariosAdAdicionados.usuarios.splice(index, 1);
          this.matDataSource.data = [... this.passoUsuariosAdAdicionados.usuarios];
        }
      });
  }

  exbibirTabelaAssinadores(): boolean {
    return this.passoUsuariosAdAdicionados !== undefined &&
      this.passoUsuariosAdAdicionados.usuarios !== undefined &&
      this.passoUsuariosAdAdicionados !== null &&
      this.passoUsuariosAdAdicionados.usuarios !== null &&
      this.passoUsuariosAdAdicionados.usuarios.length > 0;
  }

  removerPasso() {
    this.clickButton = true;
    this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
      `Tem certeza que deseja remover o passo de aprovação ${this.passoUsuariosAdAdicionados.ordem}?`)
      .then(resposta => {
        if (resposta.value) {
          let removidos = this.passoUsuariosAdAdicionados.usuarios;
          this.parentRef.removerPasso(this.passoUsuariosAdAdicionados.ordem, removidos)
        }
      });
  }

  selecaoAssinaturaDigital(row: AssinaturaUsuarioAdModel, event: any, button: any) {
    row.assinarDigitalmente = event.checked;
    this.alteradoCertificadoDigitalEvent.emit(row.assinarDigitalmente);
  }

  showCertificadoDigitalColumn(): string {
    return this.habilitarCertificadoDigital ? null : 'hidden-row';
  }
}
