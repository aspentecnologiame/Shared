import { Component, Input } from '@angular/core';
import { CienciaUsuarioAprovacaoModel } from 'src/app/saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model';

@Component({
  selector: 'app-assinatura-ciencia-acao-card-justificativa',
  templateUrl: './assinatura-ciencia-acao-card-justificativa.component.html',
  styleUrls: ['./assinatura-ciencia-acao-card-justificativa.component.scss']
})
export class AssinaturaCienciaAcaoCardJustificativaComponent   {

  @Input() dataSource: CienciaUsuarioAprovacaoModel[];

}
