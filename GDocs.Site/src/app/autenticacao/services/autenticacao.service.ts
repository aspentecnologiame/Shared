import { HttpEventType, HttpClient, HttpRequest, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import { Observable, Subject, Subscription } from 'rxjs';
import { map, last } from 'rxjs/operators';

import { CredentialModel, AccessDataModel, UserDataModel } from '../models';
import { TokenService } from './token.service';
import { environment } from './../../../environments/environment';
import { ExibicaoDeAlertaService } from './../../core/services/exibicao-de-alerta';
import { AppConfig } from '../../app.config';
import { EventMiddleService } from 'src/app/core/services/event-middle.service';

@Injectable({
  providedIn: 'root'
})
export class AutenticacaoService {

  private readonly _usuarioAutenticado = new Subject<boolean>();
  private readonly _dadosUsuarioLogado = new Subject<UserDataModel>();
  private readonly _logout = new Subject<boolean>();
  private readonly _jwtHelper = new JwtHelperService();

  private _obterTokenSubscription: Subscription;

  private _statusUsuarioAutenticado: boolean;

  public get statusUsuarioAutenticado(): boolean {
    return this._statusUsuarioAutenticado;
  }

  public get tokenEstaExpirado(): boolean {
    const dadosAcesso = this.obterDadosDeAcesso();

    if (!dadosAcesso) {
      return false;
    }
    return this._jwtHelper.isTokenExpired(dadosAcesso.accessToken);
  }

  private readonly URLS = {
    login: '/v1/Login',
    loginIntegrado: '/v1/login/integrado'
  };

  readonly URL_API: string;

  constructor(
    private readonly tokenService: TokenService,
    private readonly http: HttpClient,
    private readonly router: Router,
    protected exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private eventMiddleService: EventMiddleService
  ) {
    this.URL_API = AppConfig.settings.apisBaseUrl.gDocs;
    this.definirStatusParaAutenticacaoDoUsuario();
  }

  loginIntegrado(): Observable<AccessDataModel> {

    const req = new HttpRequest(
      'POST',
      `${this.URL_API}${this.URLS.loginIntegrado}`,
      null,
      {
        withCredentials: true
      }
    );

    return this.http.request<AccessDataModel>(req)
      .pipe(
        last(),
        map(event => this.loginResponseMap(event))
      );
  }

  login(credential: CredentialModel): Observable<AccessDataModel> {
    const req = new HttpRequest('POST', `${this.URL_API}${this.URLS.login}`, credential);

    return this.http.request<AccessDataModel>(req)
      .pipe(
        last(),
        map(event => this.loginResponseMap(event))
      );
  }

  loginGuidSignature(guid: string): Observable<AccessDataModel> {
    const credential = new CredentialModel();

    credential.grantType = 'guidSignature';
    credential.guidSignature = guid;
    return this.login(credential);
  }

  private loginResponseMap(event: HttpEvent<AccessDataModel>): AccessDataModel {
    if (event.type === HttpEventType.Response) {
      this.armazenarUsuarioAutenticado(event.body);
      return event.body;
    }
    return undefined;
  }

  private refreshToken(): void {
    const dadosAcesso = this.obterDadosDeAcesso();

    if (!dadosAcesso) {
      return;
    }

    const expiresDate = this._jwtHelper.getTokenExpirationDate(dadosAcesso.accessToken);

    const minutosRestanteValidade = (expiresDate.getTime() - (new Date()).getTime()) / 1000 / 60;

    /// Credenciais validas
    if (minutosRestanteValidade > (dadosAcesso.expiresInSeconds / 60) / 2) {
      return;
    }

    /// Credencias vencidas
    if (minutosRestanteValidade <= 0 || this._jwtHelper.isTokenExpired(dadosAcesso.accessToken)) {
      this.logout();

      this.exibirMensagemDeSessaoExpiradaENavegarParaATelaDeLogin();
      return;
    }

    const credential = new CredentialModel();
    credential.password = '';
    credential.grantType = 'refreshToken';
    credential.refreshToken = dadosAcesso.refreshToken;
    credential.userName = dadosAcesso.userData.userName;

    this._obterTokenSubscription.unsubscribe();

    this.login(credential)
      .subscribe(
        _ => { },
        _ => {
          this.exibirMensagemDeSessaoExpiradaENavegarParaATelaDeLogin();
        });
  }

  obterDadosDeAcesso(): AccessDataModel {
    return this.tokenService.obterDadosDeAcesso();
  }

  obterUsuarioLogado(): UserDataModel {
    const dadosAcesso = this.obterDadosDeAcesso();

    if (dadosAcesso === null) {
      return null;
    }

    return dadosAcesso.userData;
  }

  obterToken(): string {
    return this.tokenService.obterToken();
  }

  logout(): void {
    this._obterTokenSubscription.unsubscribe();
    this.removerAutenticado();
    this.definirStatusParaAutenticacaoDoUsuario();
    this._logout.next(true);
  }

  obterStatusUsuarioAutenticadoEvent(): Observable<boolean> {
    return this._usuarioAutenticado.asObservable();
  }

  obterDadosUsuarioEvent(): Observable<UserDataModel> {
    return this._dadosUsuarioLogado.asObservable();
  }

  obterLogoutEvent(): Observable<boolean> {
    return this._logout.asObservable();
  }

  obterPermissoes(): string[] {
    const token = this.obterToken();

    if (!token) {
      return [];
    }

    const jwt = this._jwtHelper.decodeToken(token);

    return jwt.role;
  }

  validarPermissao(permissao: string): boolean {
    return this.obterPermissoes().filter(p => p === permissao).length > 0;
  }

  private exibirMensagemDeSessaoExpiradaENavegarParaATelaDeLogin() {
    this.eventMiddleService.emitEvent(true);
    this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'A sua sessão expirou.').then(_ => {
      this.router.navigate([environment.loginUrl]);
    });
  }

  private definirStatusParaAutenticacaoDoUsuario(): void {

    const token = this.obterToken();
    if (this._jwtHelper.isTokenExpired(token)) {
      this.removerAutenticado();
    }

    const currentUser = localStorage.getItem(this.tokenService.localStorageKey);
    this._statusUsuarioAutenticado = (currentUser != null);
    this._usuarioAutenticado.next(this._statusUsuarioAutenticado);

    if (currentUser !== null) {
      const usuario = JSON.parse(currentUser);
      this._dadosUsuarioLogado.next(usuario.userData);
      this._obterTokenSubscription = this.tokenService.obterTokenEvent().subscribe(_ => this.refreshToken());
    }
  }

  private removerAutenticado() {
    localStorage.removeItem(this.tokenService.localStorageKey);
  }

  private armazenarUsuarioAutenticado(usuario: AccessDataModel) {
    localStorage.setItem(this.tokenService.localStorageKey, JSON.stringify(usuario));
    this.definirStatusParaAutenticacaoDoUsuario();
  }
}
