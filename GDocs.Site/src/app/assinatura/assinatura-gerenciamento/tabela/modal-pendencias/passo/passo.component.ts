import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { PassoAssinaturaUsuariosAdAdcionadosModel } from './../../../../../assinatura/models';

@Component({
  selector: 'app-passo',
  templateUrl: './passo.component.html',
  styleUrls: ['./passo.component.scss']
})
export class PassoComponent implements OnChanges {
  @Input() passo: PassoAssinaturaUsuariosAdAdcionadosModel;

  faIcone: string;
  corIcone: string;
  legendaIcone: string;

  ngOnChanges(changes: SimpleChanges): void {
    for (const propName in changes) {
      if (propName === 'passo' && changes[propName].currentValue) {
        this.passo = changes[propName].currentValue;
        if (!this.passo) {
          return;
        }

        const statusIcon = new Map<string, string[]>([
          ['NaoIniciado', ['', '', '']],
          ['EmAndamento', ['far fa-clock', 'icon-orange', 'Em andamento']],
          ['Concluido', ['fas fa-check', 'icon-green', 'Concluído']],
          ['ConcluidoComRejeicao', ['fa fa-window-close', 'icon-red', 'Concluído com rejeição']]
        ]);

        [this.faIcone, this.corIcone, this.legendaIcone] = statusIcon.get(this.passo.status);
        return;
      }
    }
  }
}
