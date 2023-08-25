export enum AssinaturaStatusPassoAtual {
    NaoIniciado = 1,
    Assinado = 2,
    AssinadoComObservacao = 3,
    Rejeitado = 4
  }

  export const AssinaturaStatusPassoAtualLabel = new Map<number, string>([
    [AssinaturaStatusPassoAtual.NaoIniciado, 'NaoIniciado'],
    [AssinaturaStatusPassoAtual.Assinado, 'Assinado'],
    [AssinaturaStatusPassoAtual.AssinadoComObservacao, 'AssinadoComObservacao'],
    [AssinaturaStatusPassoAtual.Rejeitado, 'Rejeitado']
  ]);
