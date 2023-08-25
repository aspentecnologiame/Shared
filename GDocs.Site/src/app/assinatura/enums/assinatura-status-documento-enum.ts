export enum AssinaturaStatusDocumento {
  Emconstrucao = 0,
  NaoIniciado = 1,
  EmAndamento = 2,
  Concluido = 3,
  ConcluidoComRejeicao = 4,
  Cancelado = 5
}

export const AssinaturaStatusDocumentoLabel = new Map<number, string>([
  [AssinaturaStatusDocumento.Emconstrucao, 'Em construção'],
  [AssinaturaStatusDocumento.NaoIniciado, 'Não iniciado'],
  [AssinaturaStatusDocumento.EmAndamento, 'Em andamento'],
  [AssinaturaStatusDocumento.Concluido, 'Assinado'],
  [AssinaturaStatusDocumento.ConcluidoComRejeicao, 'Rejeitado'],
  [AssinaturaStatusDocumento.Cancelado, 'Cancelado']
]);
