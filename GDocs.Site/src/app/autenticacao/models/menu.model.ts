export class MenuModel {
    id: number;
    icone: string;
    chave: string;
    texto: string;
    rota: string;
    idPai?: number;
    ordem: number;
    filhos?: MenuModel[];

    ativo: boolean;
}
