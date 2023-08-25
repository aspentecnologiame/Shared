import { HttpEventType } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { last, map } from 'rxjs/operators';
import { GDocsApiEndpointService } from '../core';
import { DropDownModel } from '../shared/models/dropdown.model';
import { HistoricoNotaFiscalResponseModel } from './shared/models/HistoricoNotaFiscalResponseModel';
import { ItemSaidaMaterialNF } from './shared/models/ItemSaidaMaterialNF';
import { SaidaMaterialNotaFiscalCienciaModel } from './shared/models/saida-material-nota-fiscal-ciencia-model';
import { SaidaMaterialArquivoModel } from './shared/models/SaidaMaterialArquivoModel';
import { SaidaMaterialNotaFiscal } from './shared/models/SaidaMaterialNotaFiscal';
import { SaidaMaterialNotafiscalAcaoModel } from './shared/models/SaidaMaterialNotafiscalAcaoModel';

@Injectable({
  providedIn: 'root'
})
export class SaidaMaterialNotaFiscalService {
  private readonly URLS = {
    cadastrarNovoMaterialSaidaNF:'/v1/saidamaterialnotafiscal/cadastro',
    listarNatureza:'/v1/saidamaterialnotafiscal/listarinputs/NaturezaOperacional',
    listarModalidade:'/v1/saidamaterialnotafiscal/listarinputs/ModalidadeFrete',
    listaStatusNf:'/v1/saidamaterialnotafiscal/listarinputs/Status',
    listaStatusNfFiltro:'/v1/saidamaterialnotafiscal/listarinputs/FiltroPadraoStatusPerfil',
    consultaMaterialFiltro:'/v1/saidamaterialnotafiscal/consulta',
    obterPdfPorId:'/v1/saidamaterialnotafiscal/consulta/pdf',
    ObterSaidaMaterialNfPorId:'/v1/saidamaterialnotafiscal/consulta/obtermaterialnotafiscalporid',
    cadastraArquivosUpload:'/v1/saidamaterialnotafiscal/cadastroarquivosupload',
    rdlToPdfBytesConverter: '/v1/saidamaterialnotafiscal/consulta/saidamaterialnfrdlbytesconverter',
    solicitarCancelamento:'/v1/saidamaterialnotafiscal/cancelar/CancelarOuSolicitarCancelamento',
    efetivarCancelamento:'/v1/saidamaterialnotafiscal/cancelar/EfetivarCancelamento',
    obterItemMatarialNf:'/v1/saidamaterialnotafiscal/consulta/itemMaterialnotafiscal',
    acaoMaterialNf:'/v1/saidamaterialnotafiscal/acoessaidamaterialnotafiscal',
    obterHistoricoMaterialNf: '/v1/saidamaterialnotafiscal/consulta/consultarhistorico',
    consutarCienciaNfPorId:'/v1/saidamaterialnotafiscal/ciencia/consultarcienciaporid',
    registroCienciaNf:'/v1/saidamaterialnotafiscal/ciencia/registrociencianf'
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

  consultaMaterialFiltro(parametros: any): Observable<SaidaMaterialNotaFiscal[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.consultaMaterialFiltro, parametros)
      .pipe(
        last(),
        map((event) => this.mapProcessList<SaidaMaterialNotaFiscal>(event))
      );
  }


  cadastrarMaterialSaidaNotaFiscal(saidaMaterialNf: SaidaMaterialNotaFiscal): Observable<SaidaMaterialNotaFiscal> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.cadastrarNovoMaterialSaidaNF, saidaMaterialNf)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  ObterMaterialSaidaNfPorId(id:any): Observable<SaidaMaterialNotaFiscal> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.ObterSaidaMaterialNfPorId+`?smnfIdt=${id}`)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  CancelarSaidaMaterialNF(saidaMaterialNf:SaidaMaterialNotaFiscal,motivo:string): Observable<SaidaMaterialNotaFiscal> {
    return this.gDocsApiEndpointService
    .executarRequisicaoPost(`${this.URLS.solicitarCancelamento}?idMaterialSaidaNF=${saidaMaterialNf.id}&motivo=${motivo}`)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  EfetivarCancelamentoNF(saidaMaterialNf:SaidaMaterialNotaFiscal): Observable<SaidaMaterialNotaFiscal> {
    return this.gDocsApiEndpointService
    .executarRequisicaoPost(this.URLS.efetivarCancelamento,saidaMaterialNf)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  ObterPdfNfPorId(id:any): Observable<SaidaMaterialArquivoModel[]> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.obterPdfPorId+`?idSaidaMaterial=${id}`)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  cadastraArquivosUpload( listaAssinaturaDocumentosUpload: SaidaMaterialArquivoModel[]) {
    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.cadastraArquivosUpload, listaAssinaturaDocumentosUpload,3).pipe(
      last(),
      map(event => this.mapProcess(event))
    );
  }


  acaoSaidaMaterialNf(materialSaidaAcaoModel: SaidaMaterialNotafiscalAcaoModel): Observable<SaidaMaterialNotaFiscal> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.acaoMaterialNf, materialSaidaAcaoModel)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


  obterListarItemMaterialNotaFiscal(idSaidaMaterial: any, acao:number): Observable<ItemSaidaMaterialNF[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.obterItemMatarialNf + `?idSaidaMaterial=${idSaidaMaterial}&acao=${acao}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterHistoricoMaterialNotaFiscal(idSaidaMaterial: number): Observable<HistoricoNotaFiscalResponseModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.obterHistoricoMaterialNf + `?idSaidaMaterialNotaFiscal=${idSaidaMaterial}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


  listarNaturezaOperacao(): Observable<DropDownModel[]> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.listarNatureza)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }


  listarModalidadeFrete(): Observable<DropDownModel[]> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.listarModalidade)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }


  listarStatusNotaFiscal(): Observable<DropDownModel[]> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.listaStatusNf)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  listarStatusFiltroPadrao(): Observable<DropDownModel[]> {
    return this.gDocsApiEndpointService
    .executarRequisicaoGet(this.URLS.listaStatusNfFiltro)
    .pipe(
      last(),
      map((event) => this.mapProcess(event))
    );
  }

  listarTipoSaidaMaterialNF(): Observable<DropDownModel[]> {
    return of<DropDownModel[]>([
      { id: 0, descricao: 'Sem retorno', valor: 'Sem retorno' },
      { id: 1, descricao: 'Com retorno', valor: 'Com retorno' }
    ]);
  }

  listarEscolhaFornecedor(): Observable<DropDownModel[]> {
    return of<DropDownModel[]>([
      { id: 0, descricao: 'Existente', valor: 'Existente' },
      { id: 1, descricao: 'Novo', valor: 'Novo' }
    ]);
  }

  rdlToPdfBytesConverter(parametros: any): Observable<string> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.rdlToPdfBytesConverter, parametros)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterCienciaPorId(idCienciaNf: any): Observable<SaidaMaterialNotaFiscalCienciaModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.consutarCienciaNfPorId + `?idCienciaNotaFiscal=${idCienciaNf}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


  registroCienciaNf(solicitacaoCiencia: SaidaMaterialNotaFiscalCienciaModel): Observable<SaidaMaterialNotaFiscalCienciaModel> {
    return this.gDocsApiEndpointService
    .executarRequisicaoPost(this.URLS.registroCienciaNf,solicitacaoCiencia)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


}
