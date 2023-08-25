import { PerfilModel } from './perfil.model';

export class UsuarioModel {

    constructor (private readonly id: number, public nome: string, public activeDirectoryId: string, public usuarioDeRede: string,  public perfis: PerfilModel[]) { }
}
