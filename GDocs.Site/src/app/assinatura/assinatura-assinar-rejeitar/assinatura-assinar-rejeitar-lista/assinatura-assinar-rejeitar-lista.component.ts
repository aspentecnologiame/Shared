import { Component, OnDestroy, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { interval, Subscription } from 'rxjs';
import { AppConfig } from '../../../app.config';
import { ComponentFormBase } from '../../../core/component-form-base';
import { AssinaturaService } from '../../assinatura.service';
import { DocumentoPendenteAssinaturaAbaModel, DocumentoPendenteAssinaturaModel } from '../../models';
import { AutenticacaoService } from '../../../autenticacao';
import { AssinaturaStatusDocumento } from '../../enums/assinatura-status-documento-enum';
@Component({
  selector: 'app-assinatura-assinar-rejeitar-lista',
  templateUrl: './assinatura-assinar-rejeitar-lista.component.html',
  styleUrls: ['./assinatura-assinar-rejeitar-lista.component.scss']
})
export class AssinaturaAssinarRejeitarListaComponent extends ComponentFormBase implements OnInit, OnDestroy {

  dataSource: DocumentoPendenteAssinaturaModel[];
  dataSourceMatriz: DocumentoPendenteAssinaturaModel[];
  dataSourceCiencia: DocumentoPendenteAssinaturaModel[]= [];
  readonly CONFIG_ABAS: number[] = [];
  readonly TEMPO_REFRESH_TELA: string = AppConfig.settings.tempoAtualizacaoTelaPendenciaAssinatura;
  readonly TEMPO_MAXIMO_REFRESH: number = 3600;

  ABAS_CATEGORIAS: DocumentoPendenteAssinaturaAbaModel[] = [];
  abasGroup: DocumentoPendenteAssinaturaAbaModel[] = [];
  decricaoAbaCategoria: string[] = [];
  quantidadeDocumentosOutros: number;
  quantidadeDocumentosDestaque: number;
  documentosSelecionadosLista: number[] = [];
  selectedTabIndex = 0;
  botaoAtualizarVisivel = false;
  pararTempoContador = false;


  private subscription: Subscription;

  tempoTickerEmMilissegundo = 1000;
  tempoAtual = 0;
  tempoRefreshEmSegundos = this.obterTempoEmSegundos(this.TEMPO_REFRESH_TELA);
  tempoMomentoRefreshEmSegundos = this.obterTempoEmSegundos(this.TEMPO_REFRESH_TELA);
  indicadorAlertaBarraTempo = 60;
  indicadorProgressoBarraTempo = 100;

  constructor(
    private readonly assinaturaService: AssinaturaService,
    private readonly autenticacaoService: AutenticacaoService,
  ) {
    super();
    this.CONFIG_ABAS = AppConfig.settings.configuracaoAbasPendenciaAssinatura;
  }

  ngOnInit() {
    super.ngOnInit();
    this.botaoAtualizarVisivel = false;
    this.popularListDocumentosPendentesCiencia();
    this.popularListDocumentosPendentesAssinatura();
    this.subscription = interval(this.tempoTickerEmMilissegundo)
           .subscribe(x => { this.verificarMomentoAtualizarTela(); });
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }

  public atualizarTela(): void {
    this.botaoAtualizarVisivel = false;
    this.tempoAtual = 0;
    this.indicadorProgressoBarraTempo = 100;
    this.tempoMomentoRefreshEmSegundos = this.obterTempoEmSegundos(this.TEMPO_REFRESH_TELA);
    this.popularListDocumentosPendentesCiencia();
    this.popularListDocumentosPendentesAssinatura();
  }

  private verificarMomentoAtualizarTela() {
    if(this.tempoMomentoRefreshEmSegundos <= 0)
    {
      this.atualizarTela();
    }

    if(!this.pararTempoContador)
    {
      this.tempoAtual++;
      this.indicadorProgressoBarraTempo = ((this.tempoAtual / this.tempoRefreshEmSegundos) * 100);
      this.tempoMomentoRefreshEmSegundos--;
    }
  }

  popularListDocumentosPendentesAssinatura(): void {
    this.assinaturaService.listarDocumentosPendentesAssinaturaOuJaAssinadoPeloUsuario().subscribe((response) => {
      this.dataSourceMatriz = response;
      this.configurarAbas();
      this.selectedTabIndex = this.assinaturaService.listarAbaSelecionadaArquivosPendentesDeAssinatura();
      this.onTabChanged({ index: this.selectedTabIndex });
      this.botaoAtualizarVisivel = true;
    });
  }

  popularListDocumentosPendentesCiencia(): void {
    this.dataSourceCiencia = [];
    if (this.autenticacaoService.validarPermissao('fi347:ciencia:pendente')) {
      this.assinaturaService.listarDocumentosPendentesCienciaPeloUsuario()
      .toPromise()
      .then((response) => {
        if(response != null)
        this.dataSourceCiencia.push(...response)
      });
    } else {
      this.dataSourceCiencia = null;
    }


    if (this.autenticacaoService.validarPermissao('SaidaMaterialNF:ciencia:pendente')) {
      this.assinaturaService.listarCienciasSaidaMaterialNfPorUsuario()
      .toPromise()
      .then((response) => {
        if(response != null){
          this.dataSourceCiencia.push(...response)
        }
      });
    }

    if (this.autenticacaoService.validarPermissao('fi1548:ciencia:pendente')) {
      this.assinaturaService.listarCienciasPendentesPorUsuarioFI1548()
      .toPromise()
      .then((response) => {
        if(response != null){
          this.dataSourceCiencia.push(...response)
        }
      });
    }

  }

  listaDocumentosSelecionados(listaSelecionados: number[]): void {
    this.documentosSelecionadosLista = listaSelecionados;
  }

  setarCondicaoContador(status :boolean)
  {
   this.pararTempoContador = status;
  }

  configurarAbas(): void {

    this.ABAS_CATEGORIAS = [];
    this.abasGroup = [];
    this.quantidadeDocumentosOutros = 0;
    this.quantidadeDocumentosDestaque = 0;

    const documentosDestacados = this.dataSourceMatriz.filter(documentoPendente =>
      documentoPendente.statusId == AssinaturaStatusDocumento.EmAndamento &&
      documentoPendente.destaque &&
      !documentoPendente.usuarioDaConsultaAssinou);
    if (documentosDestacados.length > 0) {
      const abaDestaque = new DocumentoPendenteAssinaturaAbaModel();
      abaDestaque.categoriaId = -1;
      abaDestaque.descricaoCategoria = 'Destaques';
      abaDestaque.quantidadeDocumentos = documentosDestacados.length;
      this.abasGroup.push(abaDestaque);
    }

    this.CONFIG_ABAS.forEach(configCategoriaId => {
      const documentos = this.dataSourceMatriz.filter(documentoPendente =>
        documentoPendente.statusId == AssinaturaStatusDocumento.EmAndamento &&
        documentoPendente.categoriaId === configCategoriaId &&
        !documentoPendente.destaque &&
        !documentoPendente.usuarioDaConsultaAssinou);

      if (documentos.length) {
        const documentoPendenteAssinaturaAba = new DocumentoPendenteAssinaturaAbaModel();
        documentoPendenteAssinaturaAba.categoriaId = documentos[0].categoriaId;
        documentoPendenteAssinaturaAba.descricaoCategoria = documentos[0].descricaoCategoria;
        documentoPendenteAssinaturaAba.quantidadeDocumentos = documentos.length;
        this.abasGroup.push(documentoPendenteAssinaturaAba);
      }
    });

    const outrosDocumentos = _.difference(
      this.dataSourceMatriz.filter(documentoPendente =>
        documentoPendente.statusId == AssinaturaStatusDocumento.EmAndamento &&
        !documentoPendente.destaque &&
        !documentoPendente.usuarioDaConsultaAssinou)
      .map(cat => cat.categoriaId), this.CONFIG_ABAS);
    if (outrosDocumentos.length) {
      this.quantidadeDocumentosOutros = outrosDocumentos.length;
    }

    if (this.quantidadeDocumentosOutros > 0) {
      const documentoAbaOutros = new DocumentoPendenteAssinaturaAbaModel();
      documentoAbaOutros.categoriaId = 0;
      documentoAbaOutros.descricaoCategoria = 'Outros';
      documentoAbaOutros.quantidadeDocumentos = this.quantidadeDocumentosOutros;
      this.abasGroup.push(documentoAbaOutros);
    }

    const documentosIncompletos = this.dataSourceMatriz.filter(documento => documento.statusId == AssinaturaStatusDocumento.EmAndamento && documento.usuarioDaConsultaAssinou);

    if (documentosIncompletos.length > 0) {
      const abaDocumentosIncompletos = new DocumentoPendenteAssinaturaAbaModel();
      abaDocumentosIncompletos.categoriaId = -2;
      abaDocumentosIncompletos.descricaoCategoria = 'Incompleto';
      abaDocumentosIncompletos.destacar = true;
      abaDocumentosIncompletos.quantidadeDocumentos = documentosIncompletos.length;
      this.abasGroup.push(abaDocumentosIncompletos)
    }

    if (this.dataSourceCiencia != null &&
        this.dataSourceCiencia != undefined &&
        this.dataSourceCiencia.length > 0) {
        const abaMeusDocumentosCiencia = new DocumentoPendenteAssinaturaAbaModel();
        abaMeusDocumentosCiencia.categoriaId = -3;
        abaMeusDocumentosCiencia.descricaoCategoria = 'Ciência';
        abaMeusDocumentosCiencia.destacar = false;
        abaMeusDocumentosCiencia.quantidadeDocumentos = this.dataSourceCiencia.length;
        this.abasGroup.push(abaMeusDocumentosCiencia)
    }

    if (this.autenticacaoService.validarPermissao('assinatura:aba:assinados')) {
      const meusDocumentosAssinados = this.dataSourceMatriz.filter(documento => documento.assinadoPorMimVisivel);

      if (meusDocumentosAssinados.length > 0) {
        const abaMeusDocumentosAssinados = new DocumentoPendenteAssinaturaAbaModel();
        abaMeusDocumentosAssinados.categoriaId = -4;
        abaMeusDocumentosAssinados.descricaoCategoria = 'Analisados Recentemente';
        abaMeusDocumentosAssinados.destacar = false;
        abaMeusDocumentosAssinados.quantidadeDocumentos = meusDocumentosAssinados.length;
        this.abasGroup.push(abaMeusDocumentosAssinados)
      }
    }

    this.ABAS_CATEGORIAS = this.abasGroup;
  }

  onTabChanged($event) {
    let selectedTabIndexAux = $event.index;
    if (this.abasGroup.length > 0) {
      if (this.abasGroup.length === 1) {
        selectedTabIndexAux = 0;
      }

      this.assinaturaService.guardarAbaSelecionadaArquivosPendentesDeAssinatura(selectedTabIndexAux);
      this.selectedTabIndex = selectedTabIndexAux;
      this.popularAbaCategoria(this.abasGroup[this.selectedTabIndex].categoriaId);
    }
  }

  popularAbaCategoria(categoriaId: number) {
    if (categoriaId === -4) {
      this.dataSource = this.dataSourceMatriz.filter(documento =>
        documento.assinadoPorMimVisivel);
      return;
    }

    if (categoriaId === -3) {
      this.dataSource = this.dataSourceCiencia;
      return;
    }

    if (categoriaId === -2) {
      this.dataSource = this.dataSourceMatriz.filter(documento =>
        documento.statusId == AssinaturaStatusDocumento.EmAndamento && documento.usuarioDaConsultaAssinou);
      return;
    }

    if (categoriaId === -1) {
      this.dataSource = this.dataSourceMatriz.filter(documento =>
        documento.statusId ==  AssinaturaStatusDocumento.EmAndamento &&
        documento.destaque && !documento.usuarioDaConsultaAssinou);
      return;
    }

    if (categoriaId === 0) {
      this.dataSource = this.dataSourceMatriz.filter(documento =>
        documento.statusId ==  AssinaturaStatusDocumento.EmAndamento &&
        this.CONFIG_ABAS.find(id => id === documento.categoriaId) === undefined &&
        !documento.destaque &&
        !documento.usuarioDaConsultaAssinou);
      return;
    }
    this.dataSource = this.dataSourceMatriz.filter(documento =>
      documento.statusId == AssinaturaStatusDocumento.EmAndamento &&
      documento.categoriaId === categoriaId && !documento.destaque &&
      !documento.usuarioDaConsultaAssinou);
  }

  obterTempoEmSegundos(tempo: string): number {
    const [horas, minutos, segundos] = tempo.split(':');
    let tempoEmSegundos = ((+horas) * 60 * 60 + (+minutos) * 60 + (+segundos));
    if(tempoEmSegundos > this.TEMPO_MAXIMO_REFRESH) {
      tempoEmSegundos = this.TEMPO_MAXIMO_REFRESH;
    }
    return tempoEmSegundos;
  }
}
