import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AppConfig } from '../../app.config';
import { PerfilModel } from '../models/perfil.model';
import { UsuarioModel } from '../models/usuario.model';
import { UsuarioService } from '../usuario.service';
import { ComponentFormBase } from './../../core/component-form-base';

@Component({
  selector: 'app-usuario-lista',
  templateUrl: './usuario-lista.component.html',
  styleUrls: ['./usuario-lista.component.scss'],
  providers: [UsuarioService]
})

export class UsuarioListaComponent extends ComponentFormBase implements OnInit {

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly router: Router,
    private readonly usuarioService: UsuarioService
  ) {
    super();
  }

  form: FormGroup;
  dataSource: UsuarioModel[];

  perfis: PerfilModel[];

  ngOnInit() {
    super.ngOnInit();
    this.carregarFormInicialPesquisarUsuario();
  }

  carregarFormInicialPesquisarUsuario() {
    this.form = this.formBuilder.group({
      nome: [''],
      perfil: [0]
    });
    this.carregarPerfis();
    this.dataSource = null;
  }

  private carregarPerfis(): void {
    this.usuarioService.listarTodosPerfis()
      .subscribe(
        resposta => {
          this.perfis = [{ id: 0, descricao: '[Todos]', peso: null, ativo: true }, ...resposta];
        },
        error => {
          console.error('Erro no método carregarUsuarios.', error);
        }
      );
  }

  resetForm(): void {
    this.carregarFormInicialPesquisarUsuario();
  }

  onSubmit() {

    this.usuarioService.listarUsuarios(this.form.value.perfil, this.form.value.nome)
      .subscribe(
        resposta => {
          this.dataSource = resposta.filter(usuPerfil => usuPerfil.perfis.length &&
            usuPerfil.perfis.filter(perfil => perfil.id !== AppConfig.settings.perfilDefaultId).length > 0);
        },
        error => {
          console.error('Erro no método carregarUsuarios.', error);
        }
      );
  }

  inserirUsuario(): void {
    this.router.navigate(['/usuario/inserir']);
  }

  alterarUsuario(usuario: UsuarioModel): void {
    localStorage.setItem('editUser', JSON.stringify(usuario));
    this.router.navigate(['/usuario/alterar/' + usuario.activeDirectoryId]);
  }
}
