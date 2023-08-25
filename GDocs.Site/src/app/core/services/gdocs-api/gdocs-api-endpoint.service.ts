import { HttpClient, HttpRequest } from '@angular/common/http';
import { AppConfig } from '../../../app.config';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class GDocsApiEndpointService {

  readonly URL_API: string;

  constructor(
    protected httpClient: HttpClient
  ) {
    this.URL_API = AppConfig.settings.apisBaseUrl.gDocs;
  }

  public executarRequisicaoPost<T>(endpoint: string, parametros = null, options = null): Observable<any> {
    const req = new HttpRequest('POST', `${this.URL_API}${endpoint}`, parametros, options);
    return this.executarRequisicao(req);
  }

  public executarRequisicaoPut<T>(endpoint: string, parametros = null, options = null): Observable<any> {
    const req = new HttpRequest('PUT', `${this.URL_API}${endpoint}`, parametros, options);
    return this.executarRequisicao(req);
  }

  public executarRequisicaoGet<T>(endpoint: string, parametros = null, options = null): Observable<any> {
    const req = new HttpRequest('GET', `${this.URL_API}${endpoint}`, parametros, options);
    return this.executarRequisicao(req);
  }

  public executarRequisicaoPatch<T>(endpoint: string, parametros = null, options = null): Observable<any> {
    const req = new HttpRequest('PATCH', `${this.URL_API}${endpoint}`, parametros, options);
    return this.executarRequisicao(req);
  }

  private executarRequisicao<T>(req: HttpRequest<T>): Observable<any> {
    return this.httpClient.request(req);
  }
}
