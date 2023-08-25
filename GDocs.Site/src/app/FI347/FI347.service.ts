import { HttpEventType } from '@angular/common/http';
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, of } from 'rxjs';
import { last, map } from 'rxjs/operators';
import { SolicitacaoCienciaModel } from '../assinatura/models/solicitacao-ciencia-model';
import { GDocsApiEndpointService } from "../core/services/gdocs-api/gdocs-api-endpoint.service";
import { CienciaEhHistoricoResponseModel } from '../saida-material-nota-fiscal/shared/models/CienciaEhHistoricoResponseModel';
import { SaidaMaterialModel } from './shared';
import { HistoricoResponseModel } from './shared/models/historicoResponseModel';
import { ItemSaidaMateialModel } from './shared/models/item-saida-material.model';
import { RegistroSaidaMaterial } from './shared/models/registro-saida-material';
import { SaidaMaterialPdfModel } from './shared/models/saida-material-pdf.model';
import { StatusSaidaMaterial } from './shared/models/status-saida-material';
import { TipoSaida } from './shared/models/tipo-saida.model';


@Injectable({
  providedIn: 'root',
})
export class Fi1347Service {
  private readonly URLS = {
    cadastrarNovoMaterialSaida:'/v1/documentofi347/cadastrar',
    cancelarSaidaMaterial: '/v1/documentofi347/consulta/cancelar',
    consultarSaidaMaterial: '/v1/documentofi347/consulta/filtrar',
    listarStatusMaterial:'/v1/documentofi347/listarstatussaidamaterial',
    consultarItemMaterial: '/v1/documentofi347/consulta/itemmaterial',
    consutarCienciaPorId:'/v1/documentofi347/ciencia/obtercienciapeloidentificador',
    consultarSolicitacaoMaterial:'/v1/documentofi347/obtermaterialporid',
    enviarMaterialAssinatura:'/v1/documentofi347/EnviarParaAssinatura',
    acaoMaterial:'/v1/documentofi347/acoessaidamateriais',
    acaoCiencia:'/v1/documentofi347/ciencia/registrociencia',
    listarStatusSaida:"/v1/documentofi347/'",
    pdf: '/v1/documentofi347/pdf',
    rdlToPdfBytesConverter: '/v1/documentofi347/solicitacoessaidamaterialrdlbytesconverter',
    historicoMaterial:'/v1/documentofi347/consulta/consultarHistorico',
    obterAprovadorCienciaRejeitada: '/v1/documentofi347/consulta/ObterRejeitadosCiencia',
  };


  dataChange: BehaviorSubject<ItemSaidaMateialModel[]> = new BehaviorSubject<ItemSaidaMateialModel[]>([]);
  dialogData: any;

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


  listarTipoSaida(): Observable<TipoSaida[]> {
    return of<TipoSaida[]>([
      { id: 0, descricao: 'Sem retorno', valor: 'Sem retorno' },
      { id: 1, descricao: 'Com retorno', valor: 'Com retorno' }
    ]);
  }


  listarStatusSaida(): Observable<StatusSaidaMaterial[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.listarStatusMaterial, null)
      .pipe(
        last(),
        map((event) => this.mapProcessList<StatusSaidaMaterial>(event))
      );
  }

  cadastrarMaterialSaida(documento: SaidaMaterialModel): Observable<SaidaMaterialModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.cadastrarNovoMaterialSaida, documento)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


  enviarMaterialAssinatura(documento: SaidaMaterialModel): Observable<SaidaMaterialModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.enviarMaterialAssinatura, documento)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  registroCiencia(solicitacaoCiencia: SolicitacaoCienciaModel): Observable<SaidaMaterialModel> {
    return this.gDocsApiEndpointService
    .executarRequisicaoPost(this.URLS.acaoCiencia,solicitacaoCiencia)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


  registroMovimentacaoMaterial(materialSaida: RegistroSaidaMaterial): Observable<SaidaMaterialModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.acaoMaterial, materialSaida)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  cancelarSaidaMaterial(
    solicitacaoSaidaMaterialId: number, motivo: string
  ): Observable<SaidaMaterialModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(`${this.URLS.cancelarSaidaMaterial}?solicitacaoSaidaMaterialId=${solicitacaoSaidaMaterialId}&motivo=${motivo}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  listarSaidaMaterial(parametros: any): Observable<SaidaMaterialModel[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoPost(this.URLS.consultarSaidaMaterial, parametros)
      .pipe(
        last(),
        map((event) => this.mapProcessList<SaidaMaterialModel>(event))
      );
  }

  obterPdf(idSaidaMaterial: any): Observable<SaidaMaterialPdfModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.pdf + `?idSaidaMaterial=${idSaidaMaterial}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterHistoricoMaterial(idSaidaMaterial: any): Observable<HistoricoResponseModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.historicoMaterial + `?idSaidaMaterial=${idSaidaMaterial}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterListarItemMaterial(idSaidaMaterial: any, acao:number): Observable<ItemSaidaMateialModel[]> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.consultarItemMaterial + `?idSaidaMaterial=${idSaidaMaterial}&acao=${acao}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }

  obterCienciaPorId(idCiencia: any): Observable<CienciaEhHistoricoResponseModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.consutarCienciaPorId + `?idSolicitacaoCiencia=${idCiencia}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }


  obterSolicitacaoMaterialPorId(ssmId: any): Observable<SaidaMaterialModel> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.consultarSolicitacaoMaterial + `?ssmId=${ssmId}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
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

  obterAprovadoresCienciaCancelada(idSaidaMaterial: any): Observable<any> {
    return this.gDocsApiEndpointService
      .executarRequisicaoGet(this.URLS.obterAprovadorCienciaRejeitada + `?idSaidaMaterial=${idSaidaMaterial}`)
      .pipe(
        last(),
        map((event) => this.mapProcess(event))
      );
  }
}
