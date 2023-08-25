export class ItemSaidaMateialModel {

  public check:boolean;
  public quantidade: string;
  public unidade: string;
  public descricao: string;
  public patrimonio: string;
  public idSolicitacaoSaidaMaterialItem: number;

  constructor(quantidade: string, unidade: string, descricao: string,patrimonio: string , idSolicitacaoSaidaMaterialItem: number = 0, check = false){
    this.quantidade = quantidade;
    this.unidade = unidade;
    this.descricao = descricao;
    this.patrimonio = patrimonio;
    this.idSolicitacaoSaidaMaterialItem = idSolicitacaoSaidaMaterialItem;
    this.check = check;
}

}
