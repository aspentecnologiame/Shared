import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { concat, Observable, of, Subject } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { PerfilModel } from '../models/perfil.model';
import { UsuarioModel } from '../models/usuario.model';
import { UsuarioAdModel } from '../models/usuarioAd.model';
import { ComponentFormBase, ExibicaoDeAlertaService } from './../../core';
import { FormValidationHelper } from './../../helpers';
import { UsuarioService } from './../usuario.service';

@Component({
  selector: 'app-usuario-cadastro',
  templateUrl: './usuario-cadastro.component.html',
  styleUrls: ['./usuario-cadastro.component.scss'],
  providers: [UsuarioService]
})

export class UsuarioCadastroComponent extends ComponentFormBase implements OnInit {

  form: FormGroup;
  _urlConsultaUsuario = '/usuario';
  _mensagemDeGravacao = 'Dados gravados com sucesso!';

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly activedRoute: ActivatedRoute,
    private readonly usuarioService: UsuarioService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService
  ) {
    super();
  }

  activeDirectoryId: string;
  perfis: PerfilModel[];
  usuariosAd: UsuarioAdModel[];
  usuariosCleanAd: UsuarioAdModel[];
  usuario: UsuarioModel;
  atualizarUsuario: boolean;
  formValidationHelper: FormValidationHelper;
  obsListaUsuario$: Observable<UsuarioAdModel[]>;
  listaUsuarioLoading = false;
  listaUsuarioinput$ = new Subject<string>();
  selectUsuarios: UsuarioAdModel[] = [];

  ngOnInit() {
    super.ngOnInit();
    this.carregarFormInicialCadastroUsuario();

    this.activeDirectoryId = this.activedRoute.snapshot.paramMap.get('id');

    if (this.activeDirectoryId !== null) {
      this.usuario = this.obterUsuarioParaEdicao();
      this.carregarValoresUsuarioNoForm();
    }
  }

  carregarFormInicialCadastroUsuario() {
    this.form = this.formBuilder.group({
      nome: new FormControl('', [Validators.required]),
      perfil: new FormControl('', [Validators.required])
    });

    this.formValidationHelper = new FormValidationHelper({
      nome: 'Nome',
      perfil: 'Perfil'
    });

    this.carregarPerfis();
    this.loadUserAD();
    this.atualizarUsuario = false;
  }

  private carregarValoresUsuarioNoForm(): void {
    this.form = this.formBuilder.group({
      nome: [this.usuario.nome.trim()],
      perfil: [this.usuario.perfis.map(({ id }) => id)]
    });
    this.atualizarUsuario = true;
  }

  private carregarPerfis(): void {
    this.usuarioService.listarTodosPerfis()
      .subscribe(
        resposta => {
          this.perfis = resposta;
        },
        error => {
          console.error('Erro no método carregarUsuarios.', error);
        }
      );
  }

  private loadUserAD() {
    this.obsListaUsuario$ = concat(
      of([]), // default items
      this.listaUsuarioinput$.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => this.listaUsuarioLoading = true),
        switchMap(term => {
          term = term.trim();
          if (term.length < 3) {
            this.listaUsuarioLoading = false;
            return of([]);
          }

          var lista = this.usuarioService.listarUsuariosActiveDirectory(term).pipe(
            catchError(() => of([])), // empty list on error
            tap(() => this.listaUsuarioLoading = false)
          );
          return lista;

        })
      )
    );
  }

  cancelarOperacao(): void {
    this.exibicaoDeAlertaService
      .exibirMensagemAviso('Atenção!', 'Operação cancelada.')
      .then(() => {
        this.router.navigate(['/usuario']);
      });
  }

  private validarFormulario(): boolean {
    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.form.controls);

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }

  gravarDados(): void {

    if (!this.validarFormulario()) {
      return;
    }

    if (this.activeDirectoryId !== null) {
      const usuarioModel = new UsuarioModel(0,
        this.usuario.nome,
        this.usuario.activeDirectoryId,
        this.usuario.nome,
        this.perfis.filter(perfil => perfil.id === this.form.value.perfil.filter(id => id === perfil.id)[0]));

      this.usuarioService.alterarUsuario(usuarioModel).subscribe(() => this.exibirMensagemDeSucesso());

    } else {

      const usuarioModel = new UsuarioModel(0,
        this.form.value.nome.nome,
        this.form.value.nome.guid,
        this.form.value.nome.usuarioDeRede,
        this.perfis.filter(perfil => perfil.id === this.form.value.perfil.filter(id => id === perfil.id)[0]));

      this.usuarioService.inserirUsuario(usuarioModel).subscribe(() => this.exibirMensagemDeSucesso());

    }
  }

  exibirMensagemDeSucesso(): void {
    this.exibicaoDeAlertaService.exibirMensagemSucesso('Sucesso!', this._mensagemDeGravacao).then(() => {
      this.router.navigate([this._urlConsultaUsuario]);
    });
  }

  private obterUsuarioParaEdicao(): UsuarioModel {
    return JSON.parse(localStorage.getItem('editUser')) as UsuarioModel;
  }
}
