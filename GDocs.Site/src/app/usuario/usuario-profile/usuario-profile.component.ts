import { Component, OnInit } from '@angular/core';
import { ComponentFormBase } from './../../core';
import { UsuarioProfileCriarAssinaturaComponent } from './modal-criar-assinatura';
import { UsuarioProfileUploadAssinaturaComponent } from './modal-upload-assinatura';
import { MatDialog } from '@angular/material';
import { UsuarioService } from '../usuario.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { AutenticacaoService, UserDataModel } from './../../autenticacao';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-usuario-profile',
  templateUrl: './usuario-profile.component.html',
  styleUrls: ['./usuario-profile.component.scss'],
  providers: [UsuarioService]
})
export class UsuarioProfileComponent extends ComponentFormBase implements OnInit {

  form: FormGroup;
  usuario: UserDataModel;

  constructor(
    public dialog: MatDialog,
    private readonly usuarioService: UsuarioService,
    private readonly domSanitizer: DomSanitizer,
    private readonly autenticacaoService: AutenticacaoService,
    private readonly formBuilder: FormBuilder,
  ) {

    super();
  }

  imagemBase64AssinaturaAtual: SafeResourceUrl;

  ngOnInit() {
    super.ngOnInit();
    this.usuario = this.autenticacaoService.obterUsuarioLogado();
    this.carregarFormInicialDadosUsuario();
    this.obterImagemAssinaturaAtual();
    this.carregarDadosUsuarioNoForm();
  }

  carregarFormInicialDadosUsuario() {
    this.form = this.formBuilder.group({
      nome: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required])
    });
  }

  private carregarDadosUsuarioNoForm(): void {
    this.form = this.formBuilder.group({
      nome: [this.usuario.fullName.trim()],
      email: [this.usuario.email.trim()]
    });
  }

  criarAssinatura() {
    const dialogRef = this.dialog.open(UsuarioProfileCriarAssinaturaComponent, {
        disableClose: true,
        height: '768',
        width: '800px'
      });

      dialogRef.afterClosed().subscribe((retornoCriarAssinatura) => {
        if(retornoCriarAssinatura) {
          this.obterImagemAssinaturaAtual();
        }
      });
  }

  uploadAssinatura() {
    const dialogRef = this.dialog.open(UsuarioProfileUploadAssinaturaComponent, {
      disableClose: true,
      height: '768',
      width: '800px'
    });

    dialogRef.afterClosed().subscribe((retornoUploadAssinatura) => {
      if(retornoUploadAssinatura) {
        this.obterImagemAssinaturaAtual();
      }
    });
  }

  obterImagemAssinaturaAtual(): void {
    this.usuarioService.obterAssinaturaBase64ArmazenadaUsuarioLogado()
    .subscribe((assinaturaBase64) => {
      if (assinaturaBase64 !== null && assinaturaBase64 !== undefined) {
        this.imagemBase64AssinaturaAtual = this.domSanitizer.bypassSecurityTrustUrl("data:image/*;base64," + assinaturaBase64);
      }
    });
  }
}
