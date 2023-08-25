import { Component, Input, SimpleChanges, OnChanges } from '@angular/core';
import { AssinaturaUsuarioAdModel } from './../../../../../../assinatura/models';

@Component({
  selector: 'app-pendencia-card',
  templateUrl: './pendencia-card.component.html',
  styleUrls: ['./pendencia-card.component.scss']
})
export class PendenciaCardComponent implements OnChanges {
  @Input() assinante: AssinaturaUsuarioAdModel;

  faIcone: string;
  corIcone: string;
  legendaIcone: string;
  exibirCanetaAssinatura = false;
  legendaIconeCanetaAssinatura = '';

  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'assinante' && changes[propName].currentValue) {
        this.assinante = changes[propName].currentValue;
        if (!this.assinante) {
          return;
        }

        const statusIcon = new Map<string, any[]>([
          ['NaoIniciado', ['', '', '', false,'']],
          ['AguardandoAssinatura', ['far fa-clock', 'icon-orange', 'Aguardando aprovação',false,'']],
          ['Assinado', ['fas fa-check', 'icon-green', 'Assinado',true,'Aprovado pelo repesentante']],
          ['Rejeitado', ['fa fa-window-close', 'icon-red', 'Rejeitado',true,'Rejeitado pelo repesentante']]
        ]);

        [this.faIcone, this.corIcone, this.legendaIcone, this.exibirCanetaAssinatura, this.legendaIconeCanetaAssinatura] = statusIcon.get(this.assinante.status);
        return;
      }
    }
  }
}
