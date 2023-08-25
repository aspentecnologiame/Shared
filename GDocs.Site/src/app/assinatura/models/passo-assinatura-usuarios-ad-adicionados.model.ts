import { AssinaturaUsuarioAdModel } from ".";

export class PassoAssinaturaUsuariosAdAdcionadosModel {
    public status: string;
    public usuarios: AssinaturaUsuarioAdModel[] = [];
    constructor(public ordem: number) { }
}
