export class DocumentoFilterModel{
    constructor(id: number, status: Array<number>, dataLiquidacao: string){
        this.id = id;
        this.status = status;
        this.dataLiquidacao = dataLiquidacao;
    }

    id: number;
    status: Array<number>;
    dataLiquidacao: string
}
