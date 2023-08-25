import { StatusAcaoMaterialNotaFiscal } from '../shared/enums/StatusAcaoMaterialNotaFiscal';
import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild, AfterViewInit, QueryList } from '@angular/core';
import { MatDialog, MatPaginator, MatRadioButton, MatRadioGroup, MatSort, MatTableDataSource } from '@angular/material';
import { BehaviorSubject } from 'rxjs';
import { ExibicaoDeAlertaService } from 'src/app/core';
import { ItemSaidaMaterialNF } from '../shared/models/ItemSaidaMaterialNF';
import { ModalAdicionarItemComponent } from './modal-adicionar-item/modal-adicionar-item.component';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-tabela-item-material',
  templateUrl: './tabela-item-material.component.html',
  styleUrls: ['./tabela-item-material.component.scss']

})
export class TabelaItemMaterialComponent implements OnInit  {

  AdicionarNovoItem:boolean = false;
  MostraCopy:boolean = false;
  MostrarInfoComplementar:boolean = false;
  MostraPagina = "hidden";
  colunasExibidas = ['quantidade','unidade', 'patrimonio','codigo','valorUnitario','tagService','descricao','acao'];
  parametroExibicao:StatusAcaoMaterialNotaFiscal;
  @Input() dataSource: ItemSaidaMaterialNF[] = [];
  @Output() eventoAtualizarDataSource = new EventEmitter<ItemSaidaMaterialNF[]>();
  @Output() eventoItemSelecionado = new EventEmitter<number>();


  @ViewChild(MatPaginator) paginador: MatPaginator;
  @ViewChild(MatSort) ordenacao: MatSort;
  dataSourceItemMaterialNF: MatTableDataSource<ItemSaidaMaterialNF> = new MatTableDataSource<ItemSaidaMaterialNF>();

  dataChange = new BehaviorSubject<ItemSaidaMaterialNF[]>([]);

  @Input()
  set configuracaoColuna(parametro: number) {
    switch (parametro) {
      case StatusAcaoMaterialNotaFiscal.Saida:
      case StatusAcaoMaterialNotaFiscal.SolicitacaoProrrogacaoNf:
        this.colunasExibidas = ['patrimonio','quantidade', 'unidade', 'descricao'];
        this.MostrarInfoComplementar = true;
        break;

      case StatusAcaoMaterialNotaFiscal.Retorno :
      case StatusAcaoMaterialNotaFiscal.SolicitacaoBaixaMaterialNFSemRetorno:
      this.colunasExibidas = ['check','patrimonio','quantidade', 'unidade', 'descricao'];
      this.MostrarInfoComplementar = true;
      break;

      case StatusAcaoMaterialNotaFiscal.HistoricoMaterial:
        this.MostrarInfoComplementar = true;
        this.colunasExibidas = ['radio','quantidade', 'unidade','descricao','status'];
        break;

      case StatusAcaoMaterialNotaFiscal.TodasColunas:
       this.colunasExibidas = ['patrimonio','quantidade', 'unidade', 'codigo', 'tagService', 'valorUnitario','descricao'];
       break;
    }
    this.parametroExibicao = parametro;
  }


  @Input()
  set AdicionarMaterial(acao:boolean) {
      this.AdicionarNovoItem = acao;
  }

  @Input()
  set mostrarClickCopy(acao:boolean) {
      this.MostraCopy = acao;
  }

  constructor(
    public dialog: MatDialog,
    private readonly currencyPipe: CurrencyPipe,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,

  ) { }



  ngOnInit() {
    this.Configuracao();
    this.AtualizarDataSource();


  }


  SelecionaPrimeiro(id):boolean{
    if(this.parametroExibicao == StatusAcaoMaterialNotaFiscal.HistoricoMaterial){

      if(this.dataSource[0] == undefined || this.dataSource[0] == null)
      return false;

       return this.dataSource[0].id == id;
    }
    return false;
  }
  montarHover(item :ItemSaidaMaterialNF):string{
    let hoverResult = `<div><strong>Valor unitário </strong><pre>${this.currencyPipe.transform(item.valorUnitario,'R$')}</pre></div>
                      <div><strong>Código </strong><pre>${item.codigo}</pre></div>`;

    if(item.tagService != null && item.tagService != undefined && item.tagService != ""  )
        hoverResult += `<div><strong>Tag Service </strong><pre>${item.tagService}</pre></div>`

    if(this.parametroExibicao == StatusAcaoMaterialNotaFiscal.HistoricoMaterial
       && (item.patrimonio != null && item.patrimonio != undefined && item.patrimonio != "" ))
        hoverResult += `<div><strong>Patrimônio </strong><pre>${item.patrimonio}</pre></div>`

   return hoverResult;
  }

   MostrarInfo():boolean{
    return this.MostrarInfoComplementar;
  }

  private Configuracao(): void {
    this.dataSourceItemMaterialNF = new MatTableDataSource<ItemSaidaMaterialNF>();
    this.dataSourceItemMaterialNF.sort = this.ordenacao;

    this.dataChange.subscribe((result) => {
      this.dataSourceItemMaterialNF.data = result;
      this.paginador.length = result.length;
      this.paginador.pageSizeOptions = [5,10, 20, 30, 40, 50, 100];
      this.paginador.pageIndex = 0;
      this.dataSourceItemMaterialNF.paginator = this.paginador;
    });

    this.EventoPersionalizadoPorAcao()
  }

  EventoPersionalizadoPorAcao(){
    if(this.parametroExibicao == StatusAcaoMaterialNotaFiscal.HistoricoMaterial){
      if(this.dataSource[0] != undefined)
      this.ItemMaterialNFCheckedRadio(this.dataSource[0].id)
    }

  }
  ngOnChanges(alteracao: SimpleChanges): void {
    for (const nomePropriedade in alteracao) {
      if (nomePropriedade === 'dataSource' && alteracao[nomePropriedade].currentValue) {
        this.dataChange.next(alteracao[nomePropriedade].currentValue);
      }
    }
  }

  MostrarCopy():boolean{
    return this.MostraCopy;
  }

  ItemMaterialNFCheckedRadio(IdMaterialItem:number){
    if(IdMaterialItem !== undefined)
    this.eventoItemSelecionado.emit(IdMaterialItem)
  }

  AdicionarNovoMaterialDialog() {
    const dialogAdicionar = this.dialog.open(ModalAdicionarItemComponent, {
      width: '600px',
      height: '600x',
      disableClose: true,
      data: {
        titulo: 'Adicionar Material'
      }
    });

    dialogAdicionar.afterClosed().subscribe(itemMaterial => {
        if(itemMaterial !== undefined){
        this.dataSource.push(itemMaterial)
        this.AtualizarDataSource();
        }
    });
  }


  EditarItemMaterialNFDialog(itemMaterial: any) {
     const dialogEditar = this.dialog.open(ModalAdicionarItemComponent, {
      width: '600px',
      height: '600x',
      disableClose: true,
      data: {
        titulo: 'Editar Material',
        item:itemMaterial
      }
    });

    dialogEditar.afterClosed().subscribe(itemMaterialNf => {
      if(itemMaterialNf !== undefined){
      const IndeceArray = this.dataSource.findIndex(x => x.codigo == itemMaterial.codigo && x.descricao == itemMaterial.descricao &&
                                                   x.quantidade == itemMaterial.quantidade && x.valorUnitario == itemMaterial.valorUnitario);
      this.dataSource[IndeceArray] = itemMaterialNf;
      this.AtualizarDataSource();
    }
    });

  }

 async DeletarItemMaterialNF(i: number, row:any)
  {
    await this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
      `Tem certeza que deseja remover o Material ${ row.descricao}?`)
      .then(result => {
        if (result.value) {
          var index = this.dataSource.findIndex(x => x.codigo == row.codigo && x.descricao == row.descricao &&
            x.quantidade == row.quantidade && x.valorUnitario == row.valorUnitario);

          this.dataSource.splice(index,1);
          this.AtualizarDataSource();
        }
      });
  }

  AtualizarDataSource()
  {
    this.dataSourceItemMaterialNF.data = this.dataSource;
    this.MostraPagina = this.dataSource.length > 0 ?"":"hidden"
    this.eventoAtualizarDataSource.emit(this.dataSource);
  }
}
