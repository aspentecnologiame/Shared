import { Component, Input } from '@angular/core';
import { CienciaUsuarioAprovacaoModel } from 'src/app/saida-material-nota-fiscal/shared/models/ciencia-usuario-aprovacao-model';

@Component({
  selector: 'app-justificativa-ciencia-rejeitada',
  templateUrl: './justificativa-ciencia-rejeitada.component.html',
  styleUrls: ['./justificativa-ciencia-rejeitada.component.scss']
})
export class JustificativaCienciaRejeitadaComponent  {


  @Input() dataSource: CienciaUsuarioAprovacaoModel[];



}
