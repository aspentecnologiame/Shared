import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AutenticacaoService } from '../services/autenticacao.service';
import { AppConfig } from './../../app.config';
import { ComponentFormBase } from './../../core/component-form-base';
import { ExibicaoDeAlertaService } from './../../core/services/exibicao-de-alerta';
import { FormValidationHelper } from './../../helpers';
import { CredentialModel } from './../models/credential.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent extends ComponentFormBase implements OnInit {

  formLogin: FormGroup;
  formValidationHelper: FormValidationHelper;

  versao: string;
  showForms = false;
  returnUrl: string;

  @HostListener('document:keyup', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent): void {

    if (event.key !== undefined &&
        event.key.toLowerCase() === 'enter') {
        this.realizarLogin();
    }
  }

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly autenticacaoService: AutenticacaoService
  ) {
    super();

    this.versao = AppConfig.settings.gDocs.Site.ProductVersion;

    if(AppConfig.settings.loginIntegrado) {
      this.autenticacaoService.loginIntegrado()
        .subscribe(
          _success => this.router.navigate(['/']),
          _error => {
            this.showForm();
          }
        );
    } else {
      this.showForm();
    }
  }

  private showForm(): void {
    this.showForms = true;

    if (this.autenticacaoService.tokenEstaExpirado) {
      this.autenticacaoService.logout();
    }
  }

  ngOnInit() {
    this.formInit();

    this.route.queryParams
      .subscribe(params => {
        this.returnUrl = params.returnUrl;
      }
    );
  }

  private formInit(): void {
    this.formLogin = this.formBuilder.group({
      username: new FormControl(null, [Validators.required]),
      credential: new FormControl(null, [Validators.required])
    });

    this.formValidationHelper = new FormValidationHelper({
      username: 'Usuário',
      credential: 'Senha'
    });
  }

  realizarLogin(): void {
    if (this.formLogin.value.credential) {
      this.formLogin.get('credential').setValue(this.formLogin.value.credential.trim());
    }

    if (this.formLogin.value.username) {
      this.formLogin.get('username').setValue(this.formLogin.value.username.trim());
    }

    if (!this.validarFormulario()) {
      return;
    }

    const credential = new CredentialModel();

    credential.grantType = 'password';
    credential.password = this.formLogin.value.credential;
    credential.userName = this.formLogin.value.username;

    this.autenticacaoService.login(credential).subscribe(
      _success => {
        if (this.returnUrl) {
          this.router.navigate([this.returnUrl]);
          return;
        }

        if (_success.userData.menu.length == 2 &&
          _success.userData.menu[0].id == 15 &&
          _success.userData.menu[1].id == 16) {
          this.router.navigate(['/fi347']);
          return;
        }

        this.router.navigate(['/']).finally(() =>
        {
          window.location.href = window.location.href;


        });
      }
    );
  }

  private validarFormulario(): boolean {
    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.formLogin.controls);

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }

}
