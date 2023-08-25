import { HttpEventType } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { last, map } from 'rxjs/operators';
import { AssinaturaUploadResponseModel } from '../assinatura/models';
import { DocumentoFI1548CienciaModel } from '../assinatura/models/documento-fi1548-ciencia-model';
import { ProcessoAssinaturaDocumentoOrigemModel } from '../assinatura/models/processo-assinatura-documento-origem-model';
import { GDocsApiEndpointService } from '../core/services/gdocs-api/gdocs-api-endpoint.service';
import { DocumentoModel } from './shared';
import { DocumentoEhAssinaturaPassosModel } from './shared/models/documento-eh-assinatura-passos';
import { DocumentoPdfModel } from './shared/models/documento-pdf.model';
import { FornecedorModel } from './shared/models/fornecedor.model';
import { ReferenciaSubstitutoModel } from './shared/models/referencia-substituto.model';
import { StatusDocumento } from './shared/models/status-documento.model';
import { TipoPagamentoModel } from './shared/models/tipo-pagamento.model';

@Injectable({
  providedIn: 'root',
})
export class Fi1548Service {
  private readonly URLS = {
    cadastrarDocumento: '/v1/documentofi1548/cadastrar',
    cancelarDocumento: '/v1/documentofi1548/consulta/cancelar',
    liquidarDocumento: '/v1/documentofi1548/consulta/liquidar',
    pdf: '/v1/documentofi1548/pdf',
    consultar: '/v1/documentofi1548/consulta/filtrar',
    rdlToPdfBytesConverter: '/v1/documentofi1548/consulta/rdltopdfbytesconverter',
    listarStatusPagamento: '/v1/documentofi1548/listarstatuspagamento',
    listarSubstitutos: '/v1/documentofi1548/listarreferenciasubstitutos',
    enviarParaAssinatura: '/v1/documentofi1548/enviarparaassinatura',
    statusPagamentoPorId: '/v1/documentofi1548/obterstatuspagamentoporid',
    passoEhUsuarioPorId: '/v1/documentofi1548/ObterPassoPorId',
    obterOrigemPorId: '/v1/documentofi1548/ObterOrigemDocumentoPorId',
    obterOrigemReferenciaSubstituto: '/v1/documentofi1548/ObterOrigemReferenciaSubstitutoDocumento',
    obterCienciaPorId: '/v1/documentofi1548/ciencia/obtercienciaporid',
    obterCienciaRejeitada: '/v1/documentofi1548/consulta/ObterRejeitadosCiencia',
    registroCienciaStatusPagamento: '/v1/documentofi1548/ciencia/registrociencia',
  };

  constructor(
    private readonly gDocsApiEndpointService: GDocsApiEndpointService
  ) { }

  private mapProcessList<T>(event: any): Array<T> {
    if (event.type === HttpEventType.Response) {
      return Object.assign([], event.body);
    }

    return undefined;
  }

  private mapProcess<T>(event: any): T {
    if (event.type === HttpEventType.Response) {
      return event.body as T;
    }
    return undefined;
  }

  listarTiposPagamento(): Observable<TipoPagamentoModel[]> {
    return of<TipoPagamentoModel[]>([
      { id: 1, descricao: 'Único', valor: 'Unico', parcelamento: false, vencimentoPara: true },
      { id: 2, descricao: 'Parcial', valor: 'Parcial', parcelamento: false, vencimentoPara: false },
      { id: 3, descricao: 'Parcelado', valor: 'Parcelado', parcelamento: true, vencimentoPara: false },
    ]);
  }

  listarFornecedores(): Observable<FornecedorModel[]> {
    return of<FornecedorModel[]>([
      { id: 1, descricao: 'Único', valor: 'Unico' },
      { id: 2, descricao: 'Diversos', valor: 'Diversos' }
    ]);
  }

  listarStatusDocumento(): Observable<StatusDocumento[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.listarStatusPagamento, null)
      .pipe(
        last(),
        map((event) => this.mapProcessList<StatusDocumento>(event))
      );
  }

  listarDocumentos(parametros: any): Observable<DocumentoModel[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.consultar, parametros)
      .pipe(
        last(),
        map((event) => this.mapProcessList<DocumentoModel>(event))
      );
  }

  listarSubstitutos(): Observable<ReferenciaSubstitutoModel[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.listarSubstitutos)
      .pipe(
        last(),
        map((event) => this.mapProcessList<ReferenciaSubstitutoModel>(event))
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

  cadastrarDocumento(documento: DocumentoModel): Observable<DocumentoModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.cadastrarDocumento, documento)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  cancelarDocumento(
    documentoId: number, motivo: string
  ): Observable<DocumentoModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(`${this.URLS.cancelarDocumento}?documentoId=${documentoId}&motivo=${motivo}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  liquidarDocumento(documento: DocumentoModel): Observable<DocumentoModel> {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.liquidarDocumento, documento).pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  obterPdf(idDocumento: any): Observable<DocumentoPdfModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.pdf + `?idDocumento=${idDocumento}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


  obterPassoEhUsuarioPorId(documentoId: any): Observable<any> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.passoEhUsuarioPorId + `?documentoId=${documentoId}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }



  enviarParaAssinatura(documento: DocumentoEhAssinaturaPassosModel): Observable<AssinaturaUploadResponseModel[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.enviarParaAssinatura, documento)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterStatusPagamentoPorId(dfi_idt: number): Observable<DocumentoModel> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.statusPagamentoPorId + `?id=${dfi_idt}`)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }


  obterOrigemPorId(dfi_idt: number): Observable<ProcessoAssinaturaDocumentoOrigemModel> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.obterOrigemPorId +`?documentoId=${dfi_idt}`)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  obterCienciaPorId(dfisoc_idt: number): Observable<DocumentoFI1548CienciaModel> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.obterCienciaPorId +`?idSolicitacaoCiencia=${dfisoc_idt}`)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  registroCienciaStatusPagamento(documentoFI1548CienciaModel: DocumentoFI1548CienciaModel): Observable<DocumentoFI1548CienciaModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.registroCienciaStatusPagamento, documentoFI1548CienciaModel)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterCienciaCancelada(documentoId: any): Observable<any> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.obterCienciaRejeitada + `?documentoId=${documentoId}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

}
