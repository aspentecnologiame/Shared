import { Component, ComponentFactoryResolver, ComponentRef, DoCheck, EventEmitter, Input, IterableDiffers, OnInit, Output, ViewChild, ViewContainerRef } from '@angular/core';
import { MatCheckbox } from '@angular/material';
import { NgSelectComponent } from '@ng-select/ng-select';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { IFeatureCategorias } from '../../../../app/shared/models/app-config-feature-categorias.model';
import { AppConfig } from '../../../app.config';
import { ExibicaoDeAlertaService } from '../../../core';
import { MatTableDataSource } from '../../../shared/mat-table-groupby/table-data-source';
import { AssinaturaService } from '../../assinatura.service';
import { AssinaturaInformacoesModel, AssinaturaUsuarioAdModel, PassoAssinaturaUsuariosAdAdcionadosModel, PassoAssinaturaUsuariosAdAdcionadosPaiModel } from '../../models';
import { AssinaturaAssinadoresCardComponent } from '../assinatura-assinadores-card/assinatura-assinadores-card.component';

@Component({
  selector: 'app-assinatura-assinadores',
  templateUrl: './assinatura-assinadores.component.html',
  styleUrls: ['./assinatura-assinadores.component.scss']
})
export class AssinaturaAssinadoresComponent implements OnInit, DoCheck {
  @ViewChild('ngselectRef')
  private ngSelectComp:NgSelectComponent;
  @ViewChild("viewContainerRef", { read: ViewContainerRef })
  VCR: ViewContainerRef;
  @ViewChild('chkIcluirDir') private chkIcluirDir: MatCheckbox;
  componentsReferences = Array<ComponentRef<AssinaturaAssinadoresCardComponent>>()
  configCategorias: IFeatureCategorias[];
  assinaturaDocumento: AssinaturaUsuarioAdModel;
  possuiMesmoPasso = true;
  mostraIncluirDir = true;
  @Input() passoAssinaturaUsuariosAdAdcionadosPai: PassoAssinaturaUsuariosAdAdcionadosPaiModel;
  @Input() assinaturaInformacoes: AssinaturaInformacoesModel;
  @Output() sinalizarAlteracaoAssinaturaDocumento = new EventEmitter();

  @Input() EditarPasso: PassoAssinaturaUsuariosAdAdcionadosPaiModel;
  @Input() IniciarAdicionandoPasso: boolean = true;



  public parentRef: AssinaturaAssinadoresComponent;
  displayedColumns: string[] = ['nome', 'assinarDigitalmente'];
  displayedColumnsDoc: string[] = ['nome','acao'];
  matDataSource: MatTableDataSource<AssinaturaUsuarioAdModel> = new MatTableDataSource<AssinaturaUsuarioAdModel>();
  passosAssinaturaUsuarioAdDirModel: PassoAssinaturaUsuariosAdAdcionadosModel[] = []
  matDataSourceDir: MatTableDataSource<PassoAssinaturaUsuariosAdAdcionadosModel>[] = []
  closed$ = new Subject<any>();
  habilitarCertificadoDigital = false;
  differ: any;

  constructor(
    private readonly CFR: ComponentFactoryResolver,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly assinaturaService: AssinaturaService,
    private differs: IterableDiffers) {
      this.differ = this.differs.find(this.matDataSource.data).create();
    }

  ngOnInit(): void {
    this.configCategorias = AppConfig.settings.categorias;
    if(this.IniciarAdicionandoPasso)
       this.adicionarNovoPasso();
    }

  ngDoCheck() {
    const change = this.differ.diff(this.matDataSource.data);
    if (change !== null && change !== undefined) {
      this.limparTextoComboAssinaturaDocumento();
      this.verificarAlteracaoAssinaturaDocumento();
     }
  }

  private limparTextoComboAssinaturaDocumento() {
    if (this.ngSelectComp !== null && this.ngSelectComp !== undefined) {
      this.ngSelectComp.handleClearClick();
    }
  }

  listarTodosUsuariosDosPassos(): AssinaturaUsuarioAdModel[] {
    let retorno: AssinaturaUsuarioAdModel[] = [];

    this.passoAssinaturaUsuariosAdAdcionadosPai.itens.forEach(item => {
      retorno.push(...item.usuarios);
    });

    if (this.chkIcluirDir.checked) {
      this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir.forEach(itemDir => {
        itemDir.usuarios.forEach(usu => {
          if (retorno.findIndex(w => w.guid === usu.guid) === -1) {
            retorno.push(usu);
          }
        });
      });
    } else {
      let retornoAux: AssinaturaUsuarioAdModel[] = [];
      retorno.forEach(item => {
        if (this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir.length === 0) {
          retornoAux.push(item);
        } else {
          this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir.forEach(dir => {
            if (dir.usuarios.findIndex(w => w.guid === item.guid) === -1) {
              retornoAux.push(item);
            }
          });
        }
      });
      retorno = retornoAux;
    }

    return retorno.filter((value,index,self)=>self.findIndex(value2=>(value2.guid === value.guid)) === index);
  }

  adicionarNovoPasso() {

    let proximoId: number;

    const componentFactory = this.CFR.resolveComponentFactory(AssinaturaAssinadoresCardComponent);

    const assinaturaAssinadoresCardComponentRef = this.VCR.createComponent(componentFactory);

    const assinaturaAssinadoresCardComponent = assinaturaAssinadoresCardComponentRef.instance;

    assinaturaAssinadoresCardComponentRef.instance.removerUsuarioAssinaturaDocumentoEvent.pipe(takeUntil(this.closed$)).subscribe((event: AssinaturaUsuarioAdModel) => {
      this.removerUsuarioListaAssinaturaDocumento(event);
      this.limparTextoComboAssinaturaDocumento();
    });

    assinaturaAssinadoresCardComponentRef.instance.alteradoCertificadoDigitalEvent.pipe(takeUntil(this.closed$)).subscribe((event: boolean) => {
      this.matDataSource.data = null;
      this.limparTextoComboAssinaturaDocumento();
      this.sinalizarAlteracaoAssinaturaDocumento.emit();
    });

    proximoId = 1
    if (this.passoAssinaturaUsuariosAdAdcionadosPai.itens.length) {
      proximoId = Math.max.apply(Math, this.passoAssinaturaUsuariosAdAdcionadosPai.itens
        .map(function (o) { return o.ordem; })) + 1;
    }

    const passoUsuariosAdAdicionados = new PassoAssinaturaUsuariosAdAdcionadosModel(proximoId);
    this.passoAssinaturaUsuariosAdAdcionadosPai.itens.push(passoUsuariosAdAdicionados);
    assinaturaAssinadoresCardComponent.passoUsuariosAdAdicionados = this.passoAssinaturaUsuariosAdAdcionadosPai.itens.filter(x => x.ordem === proximoId)[0];

    assinaturaAssinadoresCardComponent.parentRef = this;

    this.componentsReferences.push(assinaturaAssinadoresCardComponentRef);

    this.atualizarConfiguracaoCertificadoDigital();
  }

  removerAssinador(registro: AssinaturaUsuarioAdModel) {
    this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
      `Tem certeza que deseja remover o aprovador ${registro.nome}?`)
      .then(resposta => {
        if (resposta.value) {
          this.limparTextoComboAssinaturaDocumento();
          this.matDataSource.data = this.matDataSource.data.filter(item => item.guid !== registro.guid);
        }
      });
  }

  removerPasso(key: number,removidos: AssinaturaUsuarioAdModel[]) {
    this.limparTextoComboAssinaturaDocumento();
    removidos.forEach(rem => {
      if (this.matDataSource.data !== null && this.matDataSource.data !== undefined) {
        this.matDataSource.data = this.matDataSource.data.filter(item => item.guid !== rem.guid);
      }
    });

    const componentRef = this.componentsReferences.filter(
      x => x.instance.passoUsuariosAdAdicionados.ordem === key
    )[0];

    // removing from object list
    this.passoAssinaturaUsuariosAdAdcionadosPai.itens.splice(
      this.passoAssinaturaUsuariosAdAdcionadosPai.itens.indexOf(
        this.passoAssinaturaUsuariosAdAdcionadosPai.itens.filter(x => x.ordem === componentRef.instance.passoUsuariosAdAdicionados.ordem)[0]), 1);

    // reordering
    this.passoAssinaturaUsuariosAdAdcionadosPai.itens.filter(x => x.ordem > componentRef.instance.passoUsuariosAdAdicionados.ordem).forEach(passo => passo.ordem = passo.ordem - 1);

    const vcrIndex: number = this.VCR.indexOf(componentRef as any);

    // removing component from container
    this.VCR.remove(vcrIndex);

    // removing component from the list
    this.componentsReferences = this.componentsReferences.filter(
      x => x.instance.passoUsuariosAdAdicionados.ordem !== key
    );
  }

  verificarSeAssinadorJaEstaEmOutrosPassos(guid: string): boolean {
    const passoAssinaturaUsuariosAdjaAdcionados = this.passoAssinaturaUsuariosAdAdcionadosPai.itens.filter(
      passo => passo.usuarios.filter(usuario => usuario.guid === guid).length > 0);

    if (passoAssinaturaUsuariosAdjaAdcionados !== undefined && passoAssinaturaUsuariosAdjaAdcionados !== null && passoAssinaturaUsuariosAdjaAdcionados.length) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!',
        `O usuário ${passoAssinaturaUsuariosAdjaAdcionados[0].usuarios.find(usuario => usuario.guid === guid).nome}
      não pode ser adicionado, pois ele já se encontra no passo ${passoAssinaturaUsuariosAdjaAdcionados[0].ordem}`);
      return true;
    }

    return false;
  }

  adicionarDirChange(selecionado: boolean) {

    if (!selecionado) {
      this.matDataSourceDir.forEach(item => {
        item.data.forEach(x => {
          x.usuarios.forEach(y => {
            this.removerUsuarioListaAssinaturaDocumento(y)
          });
        });
      });
      this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir = [];
    }

    this.limparTextoComboAssinaturaDocumento();
    let passo = 'DIR-MESMO-PASSO';
    const categoria = this.configCategorias.find(w => w.codigo === this.assinaturaInformacoes.categoriaId);

    this.matDataSourceDir = [];

    if(categoria !== undefined && categoria.templateDir !== undefined){
      passo = categoria.templateDir;
      this.possuiMesmoPasso = false;
    }

    this.assinaturaService.listarPassosPorTag(passo).subscribe(response => {
      this.passosAssinaturaUsuarioAdDirModel = response
      response.forEach(passo => {
        const matData = new MatTableDataSource<PassoAssinaturaUsuariosAdAdcionadosModel>([passo]);
        this.matDataSourceDir.push(matData)
        this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir = []
        this.matDataSourceDir.forEach(matData => {
          this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir.push(matData.data[0])
        });
      });
    });
  }

  atualizarConfiguracaoCertificadoDigital() {
    this.componentsReferences.forEach(x => {
      x.instance.habilitarCertificadoDigital = this.assinaturaInformacoes.certificadoDigital;
      if (!this.assinaturaInformacoes.certificadoDigital) {
        x.instance.matDataSource.data.forEach(w => w.assinarDigitalmente = false);
      }
    } );

    this.habilitarCertificadoDigital = this.assinaturaInformacoes.certificadoDigital;
    if (!this.habilitarCertificadoDigital) {
      this.matDataSourceDir.forEach(
        matData => matData.data.forEach(
          passo => passo.usuarios.forEach(usuario => usuario.assinarDigitalmente = false)
        )
      )
    }
  }

  montarTextoComboAssinaturaDocumento(): string {
    return 'Selecione um dos aprovadores informados nos passos';
  }

  removerUsuarioListaAssinaturaDocumento(usuario: AssinaturaUsuarioAdModel) {
    let data = this.matDataSource.data == null ? [] : this.matDataSource.data;
    data = data.filter(w => w.guid !== usuario.guid);
    this.matDataSource.data = data;
  }

  desabilitarAssinaturaDocumento(): boolean {
    return (!this.assinaturaInformacoes.assinaturaDocumento ||this.verificarSeTemCertificadoDigital())
  }

  showCertificadoDigitalColumn(): string {
    return this.habilitarCertificadoDigital ? null : 'hidden-row';
  }

  selecaoAssinaturaDigital(row: any, event: any, button: any, matDataRef: MatTableDataSource<PassoAssinaturaUsuariosAdAdcionadosModel>) {
    this.matDataSource.data = null;
    this.limparTextoComboAssinaturaDocumento();
    this.verificarAlteracaoAssinaturaDocumento();

    if (row.assinarDigitalmente) {
        row.assinarDigitalmente = false;
    } else {
        row.assinarDigitalmente = true;
        this.sinalizarAlteracaoAssinaturaDocumento.emit();
    }
  }

  private verificarSeTemCertificadoDigital(): boolean {
    let temCertificadoDigital = false;
    let arrayUsuariosPai = [];

    this.passoAssinaturaUsuariosAdAdcionadosPai.itens.forEach(passo => {
        passo.usuarios.forEach(usu => {
          arrayUsuariosPai.push(usu);
      });
    });

    temCertificadoDigital = arrayUsuariosPai.some(x => x.assinarDigitalmente === true );

    if (!temCertificadoDigital) {
      this.passoAssinaturaUsuariosAdAdcionadosPai.itensDir.forEach(passoDir => {
        passoDir.usuarios.forEach(usuDir => {
          arrayUsuariosPai.push(usuDir);
        });
      });
      temCertificadoDigital = arrayUsuariosPai.some(x => x.assinarDigitalmente === true );
    }

    return temCertificadoDigital;
  }

  exbibirTabelaAssinadores(): boolean {
    return this.matDataSource !== undefined &&
      this.matDataSource.data !== undefined &&
      this.matDataSource !== null &&
      this.matDataSource.data !== null &&
      this.matDataSource.data.length > 0;
  }

  adicionarAssinador() {
    if (this.assinaturaDocumento === null || this.assinaturaDocumento === undefined) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'Nenhum usuário selecionado.');
      return;
    }
    let data = this.matDataSource.data == null ? [] : this.matDataSource.data;
    if (data.findIndex(w => w.guid === this.assinaturaDocumento.guid) === -1) {
      data.push(this.assinaturaDocumento);
      this.matDataSource.data = data;
    }
    this.assinaturaDocumento = null;
  }

  verificarAlteracaoAssinaturaDocumento() {
    if (! this.desabilitarAssinaturaDocumento()) {
      this.sinalizarAlteracaoAssinaturaDocumento.emit();
    }
  }

  incluirDirEhEsconderCheck(){
    this.mostraIncluirDir = false;
    this.passoAssinaturaUsuariosAdAdcionadosPai.adicionarDir =true
    this.adicionarDirChange(true)
  }

  carregarPassoPraEdicao(){
    this.passoAssinaturaUsuariosAdAdcionadosPai.itens.forEach((passo) =>{

    const novoComponentFactory = this.CFR.resolveComponentFactory(AssinaturaAssinadoresCardComponent);
    const novoAssinaturaAssinadoresCardComponentRef = this.VCR.createComponent(novoComponentFactory);
    const assinaturaAssinadoresCardComponent = novoAssinaturaAssinadoresCardComponentRef.instance;

    novoAssinaturaAssinadoresCardComponentRef.instance.removerUsuarioAssinaturaDocumentoEvent.pipe(takeUntil(this.closed$)).subscribe((event: AssinaturaUsuarioAdModel) => {
      this.removerUsuarioListaAssinaturaDocumento(event);
      this.limparTextoComboAssinaturaDocumento();
    });

    novoAssinaturaAssinadoresCardComponentRef.instance.alteradoCertificadoDigitalEvent.pipe(takeUntil(this.closed$)).subscribe((event: boolean) => {
      this.matDataSource.data = null;
      this.limparTextoComboAssinaturaDocumento();
      this.sinalizarAlteracaoAssinaturaDocumento.emit();
    });


    assinaturaAssinadoresCardComponent.passoUsuariosAdAdicionados = this.passoAssinaturaUsuariosAdAdcionadosPai.itens.filter(x => x.ordem == passo.ordem)[0]
    assinaturaAssinadoresCardComponent.parentRef = this;
    this.componentsReferences.push(novoAssinaturaAssinadoresCardComponentRef);
    this.atualizarConfiguracaoCertificadoDigital();

    assinaturaAssinadoresCardComponent.atualizarUsuariosEdicao();

  });
}
}

