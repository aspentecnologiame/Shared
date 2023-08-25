export class DocumentoPendenteAssinaturaModel {
  public id: number;
  public titulo: string;
  public descricao: string;
  public nomeDocumento: string;
  public destaque: boolean;
  public autorId: string;
  public statusId: number;
  public dataCriacao: string;
  public numero: number = null;
  public categoriaId: number;
  public descricaoCategoria: string;
  public selecionado: boolean;
  public statusPassoAtualId: number;
  public assinarCertificadoDigital: boolean;
  public assinarFisicamente: boolean;
  public usuarioDaConsultaAssinou: boolean;
  public assinadoPorMimVisivel: boolean;
  public cienciaId?: number;
}
