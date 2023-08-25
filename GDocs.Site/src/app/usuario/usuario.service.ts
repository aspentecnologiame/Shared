import { HttpEventType } from '@angular/common/http';
import { map, last } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { GDocsApiEndpointService } from '../core/services/gdocs-api/gdocs-api-endpoint.service';
import { Injectable } from '@angular/core';
import { UsuarioModel } from './models/usuario.model';
import { UsuarioAdModel } from './models/usuarioAd.model';
import { PerfilModel, AssinaturaBase64UsuarioModel } from './models';
import { AssinaturaService } from '../assinatura/assinatura.service';

@Injectable({
  providedIn: 'root'
})
export class UsuarioService {

  constructor(
    private readonly gDocsApiEndpointService: GDocsApiEndpointService,
    private readonly assinaturaService: AssinaturaService
  ) { }

  private mapProcess<T>(event: any): T {
    return event.body;
  }

  private mapProcessList<T>(event: any): Array<T> {
    if (event.type === HttpEventType.Response) {
      return Object.assign([], event.body);
    }

    return undefined;
  }

  listarUsuarios(idPerfil: number, nome: string): Observable<UsuarioModel[]> {

    idPerfil = (idPerfil == null) ? 0 : idPerfil;

    return this.gDocsApiEndpointService.executarRequisicaoGet(`/v1/Usuario/ListarUsuarios/${idPerfil}/${nome}`)
      .pipe(
        last(),
        map(event => {
          if (event.type === HttpEventType.Response) {
            return Object.assign(event.body, Array<UsuarioAdModel>());
          }
          return undefined;
        })
      );
  }

  inserirUsuario(usuarioModel: UsuarioModel): Observable<UsuarioModel> {
    return this.executarRequisicaoPost('/v1/Usuario/InserirUsuario', usuarioModel);
  }

  alterarUsuario(usuarioModel: UsuarioModel): Observable<UsuarioModel> {
    return this.executarRequisicaoPost('/v1/Usuario/AlterarUsuario', usuarioModel);
  }

  gravarAssinaturaBase64Usuario(assinaturaBase64: AssinaturaBase64UsuarioModel): Observable<any> {
    return this.gDocsApiEndpointService.executarRequisicaoPost('/v1/assinatura/gravarassinaturausuariologadoarquivobase', assinaturaBase64)
      .pipe(
        last(),
        map(event => this.mapProcess(event))
      );
  }

  obterAssinaturaBase64ArmazenadaUsuarioLogado(): Observable<string> {
    return this.assinaturaService.listarAssinaturaArmazenadaUsuarioLogado()
    .pipe(
      last(),
      map(event => {
        return event.assinaturaArmazenadaBase64;
      })
    );
  }

  private executarRequisicaoPost(url: string, usuarioModel: UsuarioModel): Observable<UsuarioModel> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(url, usuarioModel)
      .pipe(
        last(),
        map(event => this.mapProcess(event))
      );
  }

  listarUsuariosActiveDirectory(nome: string): Observable<UsuarioAdModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`/v1/Usuario/ListarUsuariosActiveDirectory/${nome}`)
      .pipe(
        last(),
        map(event => this.mapProcessList<UsuarioAdModel>(event))
      );
  }

  listarTodosPerfis(): Observable<PerfilModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet('/v1/Perfil')
      .pipe(
        last(),
        map(event => this.mapProcessList<PerfilModel>(event))
      );
  }
}
