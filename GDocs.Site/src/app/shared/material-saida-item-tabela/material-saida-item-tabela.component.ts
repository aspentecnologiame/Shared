import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { MatDialog, MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { BehaviorSubject } from 'rxjs';
import { ExibicaoDeAlertaService } from 'src/app/core';
import { MaterialSaidaModalComponent } from 'src/app/shared/material-saida-modal/material-saida-modal.component';
import { StatusAcaoMaterial } from '../../FI347/shared/enums/status-acao-materia';
import { ItemSaidaMateialModel } from '../../FI347/shared/models/item-saida-material.model';



@Component({
  selector: 'app-material-saida-item-tabela',
  templateUrl: './material-saida-item-tabela.component.html',
  styleUrls: ['./material-saida-item-tabela.component.scss']
})
export class MaterialSaidaItemTabelaComponent implements OnInit {

  AdicionarItem:boolean = false;
  MostraPag = "hidden";
  displayedColumns = ['patrimonio','quantidade', 'unidade', 'descricao','acao'];
  MostrarInfoDados:boolean = false;
  parametroConfColuna:StatusAcaoMaterial;
  @Input() dataSource: ItemSaidaMateialModel[] = [];
  @Output() eventoItemMaterial = new EventEmitter<ItemSaidaMateialModel[]>();
  @Output() eventoItemMarcado = new EventEmitter<number>();


  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  dataSourceMaterial: MatTableDataSource<ItemSaidaMateialModel> = new MatTableDataSource<ItemSaidaMateialModel>();

  dataChange = new BehaviorSubject<ItemSaidaMateialModel[]>([]);

  @Input()
  set configuracao(parametro: number) {
    switch (parametro) {
      case StatusAcaoMaterial.RegistroSaida:
      case StatusAcaoMaterial.SolicitacaoProrrogacao:
        this.displayedColumns = ['patrimonio','quantidade', 'unidade', 'descricao'];
        break;

      case StatusAcaoMaterial.RegistroRetorno :
      case StatusAcaoMaterial.SolicitacaoBaixaMaterialSemRetorno:
      this.displayedColumns = ['check','patrimonio','quantidade', 'unidade', 'descricao'];
      break;

      case StatusAcaoMaterial.HistoricoMaterial:
        this.MostrarInfoDados = true
        this.displayedColumns = ['radio','quantidade', 'unidade', 'descricao','status'];
        break;
    }
    this.parametroConfColuna = parametro;
  }


  @Input()
  set Adicionar(acao:boolean) {
      this.AdicionarItem = acao;
  }

  constructor(
    public dialog: MatDialog,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,

  ) { }


  ngOnInit() {
    this.ConfigurarTabela();
    this.AtualizarTabela();
  }


  private ConfigurarTabela(): void {
    this.dataSourceMaterial = new MatTableDataSource<ItemSaidaMateialModel>();
    this.dataSourceMaterial.sort = this.sort;

    this.dataChange.subscribe((data) => {
      this.dataSourceMaterial.data = data;
      this.paginator.length = data.length;
      this.paginator.pageSizeOptions = [5,10, 20, 30, 40, 50, 100];
      this.paginator.pageIndex = 0;
      this.dataSourceMaterial.paginator = this.paginator;
    });

    this.EventoIniciarHistorico();
  }

  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'dataSource' && changes[propName].currentValue) {
        this.dataChange.next(changes[propName].currentValue);
      }
    }
  }

  RadioButtonMarcado(IdItem:number){
    if(IdItem !== undefined)
    this.eventoItemMarcado.emit(IdItem)
  }

  AdicicaoMaterialDialog() {
    const dialogRef = this.dialog.open(MaterialSaidaModalComponent, {
      width: '600px',
      height: '600x',
      disableClose: true,
      data: {
        titulo: 'Adicionar Material'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
        if(result !== undefined){
        this.dataSource.push(result)
        this.AtualizarTabela();
        }
    });
  }


  EditarMaterialDialog(itemMaterial: any) {
     const dialogRef = this.dialog.open(MaterialSaidaModalComponent, {
      width: '600px',
      height: '600x',
      disableClose: true,
      data: {
        titulo: 'Editar Material',
        editarItem:itemMaterial
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if(result !== undefined){
      const foundIndex = this.dataSource.findIndex(x => x.patrimonio == itemMaterial.patrimonio && x.descricao == itemMaterial.descricao && x.quantidade == itemMaterial.quantidade);
      this.dataSource[foundIndex] = result;
      this.AtualizarTabela();
    }
    });

  }

  async DeletarItem(i: number, row:any)
  {
   await this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
      `Tem certeza que deseja remover o Material ${ row.descricao}?`)
      .then(resposta => {
        if (resposta.value) {
          const foundIndex = this.dataSource.findIndex(x => x.patrimonio == row.patrimonio && x.descricao == row.descricao && x.quantidade == row.quantidade);
          this.dataSource.splice(foundIndex,1);
          this.AtualizarTabela();
        }
      });
  }

  AtualizarTabela()
  {
    this.dataSourceMaterial.data = this.dataSource;
    this.MostraPag = this.dataSource.length > 0 ?"":"hidden"
    this.eventoItemMaterial.emit(this.dataSource);
  }
  montarHover(item :ItemSaidaMateialModel):string{
     return`<div><strong>Patrimônio </strong><pre>${item.patrimonio}</pre></div>`
  }

  MostrarInfoDado(row:ItemSaidaMateialModel):boolean{
    return(this.MostrarInfoDados && row.patrimonio != null && row.patrimonio != undefined && row.patrimonio != "" )
  }
  EventoIniciarHistorico(){
    if(this.parametroConfColuna == StatusAcaoMaterial.HistoricoMaterial){
      if(this.dataSource[0] != undefined)
      this.RadioButtonMarcado(this.dataSource[0].idSolicitacaoSaidaMaterialItem)
    }
  }

  SelecionaPrimeiroItem(idItem):boolean{
      if(this.parametroConfColuna == StatusAcaoMaterial.HistoricoMaterial){

        if(this.dataSource[0] == undefined || this.dataSource[0] == null)
        return false;

       return this.dataSource[0].idSolicitacaoSaidaMaterialItem == idItem;
      }
    return false;
  }
}
