import { HttpEventType } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { catchError, last, map } from 'rxjs/operators';
import { ExibicaoDeAlertaService, GDocsApiEndpointService } from '../core';
import {
  AssinaturaArmazenadaUsuarioModel,
  AssinaturaArquivoAssinaturaUsuarioAdModel,
  AssinaturaCategoriaModel,
  AssinaturaInformacoesModel,
  AssinaturaModel,
  AssinaturaPassoItemAssinarRejeitarModel,
  AssinaturaUploadResponseModel,
  AssinaturaUsuarioAdModel, DocumentoPendenteAssinaturaModel, InformacoesDocumentoEnviadoAssinaturaUploadResponseModel, PassoAssinaturaUsuariosAdAdcionadosModel
} from './models';
import { AssinaturaAssinanteRepresentanteModel } from './models/assinatura-assinante-representante.model';
import { GravarCachePassoModel } from './models/assinatura-gravar-cache-passo.model';
import { AssinaturaInformacoesStatusDocumento } from './models/assinatura-informacaoes-status-documento.model';

@Injectable({
  providedIn: 'root'
})
export class AssinaturaService {
  constructor(
    private readonly gDocsApiEndpointService: GDocsApiEndpointService,
    private readonly router: Router,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
  ) { }

  private readonly URLS = {
    upload: '/v1/upload',
    adicionar: '/v1/assinatura/gravar',
    cancelarProcesso: '/v1/assinatura/cancelarprocesso',
    adicionarArquivos: '/v1/assinatura/gravararquivosupload',
    gerenciamentoFiltrar: '/v1/assinatura/gerenciamento/filtrar',
    obterReferenciaSubstitutoDocumento: '/v1/assinatura/obterreferenciasubstitutodocumento',
    atribuirRepresentantes: '/v1/assinatura/atribuirrepresentante',
    listarAssinanteRepresentanteProcesso: '/v1/assinatura/listarassinantesprocesso',
    obterProcessoComPassosParaAssinatura: '/v1/assinatura/obterprocessocompassosparaassinatura',
    listarNomesDocumentos: '/v1/assinatura/listarnomesdocumento',
    listarArquivosUploadPorPadId: '/v1/assinatura/listarArquivosUpload',
    listarArquivosUploadEPassoUsuario: '/v1/assinatura/ListarArquivosUploadEPassoUsuario',
    assinaturaUsuarioAd: '/v1/assinatura/usuario/listarusuariosactivedirectory/',
    listarDocumentosPendentesAssinaturaOuJaAssinadosPeloUsuario: '/v1/assinatura/listardocumentospendentesassinaturaoujaassinadospelousuario',
    listardocumentospendentescienciapelousuario: '/v1/documentofi347/ciencia/listardocumentospendentescienciapelousuario',
    listarCienciasSaidaMaterialNfPorUsuario: '/v1/SaidaMaterialNotaFiscal/ciencia/listarcienciaspendente',
    listarAssinaturaCategoriaExclude: '/v1/assinatura/listarAssinaturaCategoriaExclude',
    assinar: '/v1/assinatura/assinar',
    rejeitar: '/v1/assinatura/rejeitar',
    listarAssinaturaArmazenadaUsuarioLogado: '/v1/assinatura/listarAssinaturaArmazenadaUsuarioLogado',
    assinarrejeitarvalidarexisteassinaturadocjaassinado: '/v1/assinatura/assinarrejeitarvalidarexisteassinaturadocjaassinado',
    atualizarDestaque: '/v1/assinatura/atualizardestaque',
    rdlToPdfBytesConverter: '/v1/assinatura/gerenciamentordlbytesconverter',
    gerarNumeracaoAutomatica: '/v1/numeracaoautomatica',
    listarPassosPorTag: '/v1/assinatura/listarpassosportag',
    gravarFareiEnvio: '/v1/assinatura/gravarfareienvio',
    baixarDococumento: '/v1/assinatura/listararquivoprocessocompleto',
    gravarCacheDownLoad: '/v1/cache/gravarpassocache',
    listarCacheDownLoad: '/v1/cache/listarpassocache',
    limparCacheDownload: '/v1/cache/limparpassocache',
    listarCienciasPendentesPorUsuarioFI1548: '/v1/documentofi1548/ciencia/listarcienciaspendentesporusuario',
    obterDocumentoFI1548PorNumero: '/v1/assinatura/obterdocumentosubstituido',
    obterMotivoCancelamento: '/v1/assinatura/obtermotivocancelamento'
  };

  private readonly localStorageKeys = {
    arquivosPendentesDeAssinaturaJaVisualizados: "arquivospendentesdeassinatura.javisualizados",
    arquivosPendentesDeAssinaturaAbaSelecionada: "arquivospendentesdeassinatura.abaselecionada"
  };

  guardarAbaSelecionadaArquivosPendentesDeAssinatura(abaSelecionada: number): void {
    localStorage.setItem(this.localStorageKeys.arquivosPendentesDeAssinaturaAbaSelecionada, abaSelecionada.toString());
  }

  listarAbaSelecionadaArquivosPendentesDeAssinatura(): number {
    const abaSelecionada = localStorage.getItem(this.localStorageKeys.arquivosPendentesDeAssinaturaAbaSelecionada);
    if (abaSelecionada === null || abaSelecionada === '') {
      return 0;
    }

    return abaSelecionada as unknown as number;
  }

  marcarArquivoPendenteDeAssinaturaComoJaVisualizado(processoAssinaturaDocumentoId: number): void {
    const arquivosLidos = this.listarArquivosPendentesDeAssinaturaJaVisualizados();
    arquivosLidos.push(processoAssinaturaDocumentoId)
    localStorage.setItem(this.localStorageKeys.arquivosPendentesDeAssinaturaJaVisualizados, JSON.stringify(arquivosLidos));
  }

  removerArquivoPendenteDeAssinaturaJaVisualizado(processoAssinaturaDocumentoId: number): void {
    const arquivosLidos = this.listarArquivosPendentesDeAssinaturaJaVisualizados();
    const index = arquivosLidos.indexOf(processoAssinaturaDocumentoId);
    if (index > -1) {
      arquivosLidos.splice(index, 1);
    }
    localStorage.setItem(this.localStorageKeys.arquivosPendentesDeAssinaturaJaVisualizados, JSON.stringify(arquivosLidos));
  }

  listarArquivosPendentesDeAssinaturaJaVisualizados(): number[] {
    const arquivosLidos = localStorage.getItem(this.localStorageKeys.arquivosPendentesDeAssinaturaJaVisualizados);
    if (arquivosLidos === null || arquivosLidos === '') {
      return [] as number[];
    }

    return JSON.parse(arquivosLidos) as number[];
  }

  exibirMensagemPendenciasAssinaturaArmazenadaNaoEncontrada(): void {
    this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
      'Não é possível realizar a assinatura, pois você não possui uma assinatura cadastrada no sistema. <br>' +
      'Deseja ser redirecionado(a) para os <b>Meus dados</b> para cadastrá-la?'
    )
      .then(resposta => {
        if (resposta.value) {
          this.router.navigate(['/usuario-profile/me/']);
        }
        else {
          this.router.navigate(['assinatura/pendencias']);
        }
      });
  }

  exbirMensagemNaoEPossivelAssinar(): void {
    this.exibicaoDeAlertaService
      .exibirMensagemAviso('Atenção!', 'Não é possível aprovar/rejeitar esse documento, pois ele não se encontra mais como pendente de aprovação.')
      .then(() => {
        this.router.navigate(['assinatura/pendencias']);
      });
  }

  exbirMaisAprovadoresPendentes(): void {
    this.exibicaoDeAlertaService
      .exibirMensagemAviso('Atenção!', 'Possui usuários pendentes para realizar a assinatura. Você receberá um e-mail no final do processo com documento aprovado.')
  }

  exbirMensagemNaoEhPosivelFazerDownload(GravarCachePassoModel:GravarCachePassoModel): void {
    this.exibicaoDeAlertaService
      .exibirMensagemAviso('Atenção!', `Esse documento está pendente de aprovação com  ${GravarCachePassoModel.nomeUsuario}. Portanto não será possível fazer o download ou rejeitar o documento.`)
  }
  enviarDocumento(file: File): Observable<InformacoesDocumentoEnviadoAssinaturaUploadResponseModel> {
    const formData = new FormData();
    formData.append('file', file);
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.upload, formData, { reportProgress: true })
      .pipe(
        map(event => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              return new InformacoesDocumentoEnviadoAssinaturaUploadResponseModel(true, Math.round(100 * event.loaded / event.total));

            case HttpEventType.Response:
              return new InformacoesDocumentoEnviadoAssinaturaUploadResponseModel(false, 100, Object.assign(new AssinaturaUploadResponseModel(), event.body));

            default:
              return undefined;
          }
        })
      );
  }

  listarUsuariosAdParaAssinaturaPorNome(nome: string): Observable<AssinaturaUsuarioAdModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.assinaturaUsuarioAd}${nome}`)
      .pipe(
        last(),
        map(event => this.mapList<AssinaturaUsuarioAdModel>(event))
      );
  }

  listarGerenciamento(filtro: any): Observable<AssinaturaInformacoesModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(`${this.URLS.gerenciamentoFiltrar}`, filtro)
      .pipe(
        last(),
        map(event => this.mapList<AssinaturaInformacoesModel>(event))
      );
  }

  obterReferenciaSubstitutoDocumento(processoAssinaturaDocumentoOrigemId: any): Observable<AssinaturaInformacoesModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.obterReferenciaSubstitutoDocumento, processoAssinaturaDocumentoOrigemId)
      .pipe(
        last(),
        map(event => this.mapList<AssinaturaInformacoesModel>(event))
      );
  }

  listarPassoAssinanteRepresentanteProcesso(processoAssinaturaDocumentoId: number ,  status :boolean): Observable<AssinaturaAssinanteRepresentanteModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarAssinanteRepresentanteProcesso}/${processoAssinaturaDocumentoId}/${status}`)
      .pipe(
        last(),
        map(event => this.mapList<AssinaturaAssinanteRepresentanteModel>(event))
      );
  }

  obterProcessoComPassosParaAssinatura(processoAssinaturaDocumentoId: number): Observable<AssinaturaModel> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.obterProcessoComPassosParaAssinatura}/${processoAssinaturaDocumentoId}`)
      .pipe(
        last(),
        map(event => this.mapProcess<AssinaturaModel>(event))
      );
  }

  atualizarDestaque(assinaturaInformacoesModel: AssinaturaInformacoesModel) : Observable<number> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.atualizarDestaque, assinaturaInformacoesModel).pipe(
      last(),
      map(event => this.mapProcess(event))
    );
  }

  listarNomesDocumentos(termo: string): Observable<any> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarNomesDocumentos}/${termo}`)
      .pipe(
        last(),
        map(event => this.mapList<any>(event))
      );
  }

  obterDocumentoFI1548(numeroDocumento: number): Observable<any> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.obterDocumentoFI1548PorNumero}/${numeroDocumento}`)
      .pipe(
        last(),
        map(event => this.mapList<any>(event))
      );
  }

  atribuirRepresentantes(model: any[]): Observable<AssinaturaAssinanteRepresentanteModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.atribuirRepresentantes, Object.assign({}, { assinantes: [...model] }))
      .pipe(
        last(),
        map(event => this.mapList(event))
      );
  }

  listarStatusDocumento(): Observable<AssinaturaInformacoesStatusDocumento[]> {
    return of<AssinaturaInformacoesStatusDocumento[]>([
      { id: 0, descricao: 'Em construção' },
      { id: 1, descricao: 'Não iniciado' },
      { id: 2, descricao: 'Em andamento' },
      { id: 3, descricao: 'Concluído' },
      { id: 4, descricao: 'Concluído com rejeição' },
      { id: 5, descricao: 'Cancelado' }
    ]);
  }

  listarArquivosUploadPorPadId(padId: number, listarTodos = false): Observable<AssinaturaUploadResponseModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarArquivosUploadPorPadId}/${padId}?listartodos=${listarTodos}`)
      .pipe(
        last(),
        map(event => this.mapList<AssinaturaUploadResponseModel>(event))
      );
  }

  listarArquivosUploadEPassoUsuario(padId: number): Observable<AssinaturaArquivoAssinaturaUsuarioAdModel> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarArquivosUploadEPassoUsuario}/${padId}`)
      .pipe(
        last(),
        map(event => this.mapProcess(event))
      );
  }

  listarAssinaturaCategoriaExclude(exclude = true): Observable<AssinaturaCategoriaModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarAssinaturaCategoriaExclude}/${exclude}`)
      .pipe(
        last(),
        map(event => this.mapList<AssinaturaCategoriaModel>(event))
      );
  }

  gravar(assinaturaModel: AssinaturaModel): Observable<AssinaturaModel> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.adicionar, assinaturaModel).pipe(
      last(),
      map(event => this.mapProcess(event))
    );
  }

  cancelarProcesso(assinaturaInformacaoModel: AssinaturaInformacoesModel): Observable<AssinaturaInformacoesModel> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.cancelarProcesso, assinaturaInformacaoModel).pipe(
      last(),
      map(event => this.mapProcess(event))
    );
  }

  gravarArquivos(processoAssinaturaDocumentoId: number, listaAssinaturaDocumentosUpload: AssinaturaUploadResponseModel[]) {
    listaAssinaturaDocumentosUpload.forEach(a => a.processoAssinaturaDocumentoId = processoAssinaturaDocumentoId);
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.adicionarArquivos, listaAssinaturaDocumentosUpload).pipe(
      last(),
      map(event => this.mapProcess(event))
    );
  }

  assinar(assinaturaPassoItemRejeitarModel: AssinaturaPassoItemAssinarRejeitarModel): Observable<any> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.assinar, assinaturaPassoItemRejeitarModel).pipe(
      last(),
      map(event => this.mapProcess(event))
    );
  }

  GravarCacheDownLoad(padid): Observable<any> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.gravarCacheDownLoad,padid).pipe(
      last(),
      map(event => this.mapProcess(event)),
      catchError((err) => {
        return throwError(err);
      })
    );
  }

    ListarCacheDownload(padId: number): Observable<GravarCachePassoModel> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarCacheDownLoad}?padId=${padId}`)
      .pipe(
        last(),
        map(event => this.mapProcess<GravarCachePassoModel>(event)),
        catchError((err) => {
          return throwError(err);
        })
      );
  }


  BaixarDocumento(padId: number): Observable<AssinaturaUploadResponseModel> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.baixarDococumento}/${padId}`)
      .pipe(
        last(),
        map(event => this.mapProcess<AssinaturaUploadResponseModel>(event)),
        catchError((err) => {
          return throwError(err);
        })
      );
  }

  GravarFareiEnvio(padId: number , fareiEnvio:boolean): Observable<any> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(`${this.URLS.gravarFareiEnvio}?padId=${padId}&fareiEnvio=${fareiEnvio}`)
      .pipe(
        last(),
        catchError((err) => {
          return throwError(err);
        })
      );
  }

  LimparCacheDownload(padId: number): Observable<GravarCachePassoModel> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.limparCacheDownload,padId)
      .pipe(
        last(),
        map(event => this.mapProcess<GravarCachePassoModel>(event)),
        catchError((err) => {
          return throwError(err);
        })
      );
  }

  rejeitar(assinaturaPassoItemRejeitarModel: AssinaturaPassoItemAssinarRejeitarModel): Observable<any> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.rejeitar, assinaturaPassoItemRejeitarModel).pipe(
      last(),
      map(event => this.mapProcess(event))
    );
  }

  assinarRejeitarValidarExisteAssinaturaDocJaAssinado(listaDeprocessoAssinaturaDocumentoId: number[]): Observable<boolean> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.assinarrejeitarvalidarexisteassinaturadocjaassinado, listaDeprocessoAssinaturaDocumentoId)
      .pipe(
        last(),
        map(event => this.mapProcess<boolean>(event)),
        catchError((err) => {
          const httpStatusCodeAssinaturaNaoEncontrada = 404;
          if (err.status === httpStatusCodeAssinaturaNaoEncontrada) {
            this.exibirMensagemPendenciasAssinaturaArmazenadaNaoEncontrada();
          }
          else {
            this.exbirMensagemNaoEPossivelAssinar();
          }
          return throwError(err);
        })
      );
  }

  obterMotivoCancelamento(documentoId: any): Observable<any> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.obterMotivoCancelamento + `?documentoId=${documentoId}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }



  listarAssinaturaArmazenadaUsuarioLogado(): Observable<AssinaturaArmazenadaUsuarioModel> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarAssinaturaArmazenadaUsuarioLogado}`)
      .pipe(
        last(),
        map(event => this.mapProcess<AssinaturaArmazenadaUsuarioModel>(event)),
        catchError((err) => {
          return throwError(err);
        })
      );
  }

  listarDocumentosPendentesAssinaturaOuJaAssinadoPeloUsuario(): Observable<DocumentoPendenteAssinaturaModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarDocumentosPendentesAssinaturaOuJaAssinadosPeloUsuario}`)
      .pipe(
        last(),
        map(event => this.mapList<DocumentoPendenteAssinaturaModel>(event))
      );
  }

  listarDocumentosPendentesCienciaPeloUsuario(): Observable<DocumentoPendenteAssinaturaModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listardocumentospendentescienciapelousuario}`)
      .pipe(
        last(),
        map(event => this.mapList<DocumentoPendenteAssinaturaModel>(event))
      );
  }

  listarCienciasSaidaMaterialNfPorUsuario(): Observable<DocumentoPendenteAssinaturaModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarCienciasSaidaMaterialNfPorUsuario}`)
      .pipe(
        last(),
        map(event => this.mapList<DocumentoPendenteAssinaturaModel>(event))
      );
  }

  listarCienciasPendentesPorUsuarioFI1548(): Observable<DocumentoPendenteAssinaturaModel[]> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarCienciasPendentesPorUsuarioFI1548}`)
      .pipe(
        last(),
        map(event => this.mapList<DocumentoPendenteAssinaturaModel>(event))
      );
  }

  rdlToPdfBytesConverter(parametros: any): Observable<string> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.rdlToPdfBytesConverter, parametros)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  gerarNumeracaoAutomatica(categoriaId: number): Observable<number> {
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.gerarNumeracaoAutomatica}?categoriaId=${categoriaId}`)
      .pipe(
        last(),
        map(event => this.mapProcess<number>(event)),
        catchError((err) => {
          return throwError(err);
        })
      );
  }

  listarPassosPorTag(tag: string) : Observable<PassoAssinaturaUsuariosAdAdcionadosModel[]>{
    return this.gDocsApiEndpointService.executarRequisicaoGet(`${this.URLS.listarPassosPorTag}/${tag}`)
      .pipe(
        last(),
        map(event => this.mapList<PassoAssinaturaUsuariosAdAdcionadosModel>(event))
      );
  }

  private mapProcess<T>(event: any): T {
    if (event.type === HttpEventType.Response) {
      return event.body as T;
    }
    return undefined;
  }

  private mapList<T>(event: any): Array<T> {
    if (event.type !== HttpEventType.Response) {
      return undefined;
    }
    return Object.assign([], event.body);
  }
}
