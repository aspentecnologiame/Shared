import { Component, Input } from '@angular/core';
import { MotivoCancelamentoModel } from 'src/app/saida-material-nota-fiscal/shared/models/motivo-cancelamento-status-pagamento-model';

@Component({
  selector: 'app-motivo-cancelamento',
  templateUrl: './motivo-cancelamento.component.html',
  styleUrls: ['./motivo-cancelamento.component.scss']
})
export class MotivoCancelamentoComponent  {


  @Input() dataSource: MotivoCancelamentoModel;

 



}
