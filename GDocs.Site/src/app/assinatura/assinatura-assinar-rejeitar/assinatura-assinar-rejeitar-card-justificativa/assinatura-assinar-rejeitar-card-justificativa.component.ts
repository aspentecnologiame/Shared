import { Component, Input, OnInit } from '@angular/core';
import { AssinaturaStatusPassoAtual } from '../../enums/assinatura-status-passo-atual-enum';
import { AssinaturaUsuarioAdModel } from '../../models/assinatura-usuario-ad.model';
import { AssinaturaAssinarRejeitarAcaoBaseComponent } from '../assinatura-assinar-rejeitar-acao-base/assinatura-assinar-rejeitar-acao-base.component';

@Component({
  selector: 'app-assinatura-assinar-rejeitar-card-justificativa',
  templateUrl: './assinatura-assinar-rejeitar-card-justificativa.component.html'
})
export class AssinaturaAssinarRejeitarCardJustificativaComponent extends AssinaturaAssinarRejeitarAcaoBaseComponent  implements OnInit{

  autor:string;
  temJustificasParaSeremExibidas = false;
  aprovacaoAd:AssinaturaUsuarioAdModel[] = []
  @Input()
  set NomeAutor(nome:string) {
      this.autor = nome;
  }

  @Input()
  set DataSource(AprovandorJustificativa:AssinaturaUsuarioAdModel[]) {
    this.aprovacaoAd = AprovandorJustificativa;
    this.temJustificasParaSeremExibidas = this.aprovacaoAd !== null && this.aprovacaoAd.length > 0;
  }

  retornaClassStatusPasso(statusPasso: string): string {

    if(statusPasso === this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.Rejeitado)){
      return "marcador-rejeitado";
    }

    return "marcador-assinadocomobservacao";
  }

  retornaDescricaoStatusPasso(statusPasso: string): string {

    if(statusPasso ===  this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.Rejeitado)){
      return "Rejeitado";
    }

    return this.textoAssinaturaComObs;
  }

  retornaTextoAssinatura(statusPasso: string){
    if(statusPasso === this.retornarDescricaoStatusPassoAtual(AssinaturaStatusPassoAtual.Rejeitado)){
      return "Rejeitado";
    }

    return "Aprovado";
  }

}
