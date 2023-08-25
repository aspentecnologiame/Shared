import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { AssinaturaService } from 'src/app/assinatura/assinatura.service';
import { AssinaturaAssinanteRepresentanteModel } from 'src/app/assinatura/models/assinatura-assinante-representante.model';
import { ExibicaoDeAlertaService } from 'src/app/core';

@Component({
  selector: 'app-assinatura-gerenciamento-representante',
  templateUrl: './modal-representante.component.html',
  styleUrls: ['./modal-representante.component.scss']
})
export class AssinaturaGerenciamentoRepresentanteComponent implements OnInit {
  dataSource: AssinaturaAssinanteRepresentanteModel[];

  constructor(
    private readonly assinaturaService: AssinaturaService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    public dialogRef: MatDialogRef<AssinaturaGerenciamentoRepresentanteComponent>,
    @Inject(MAT_DIALOG_DATA) public assinaturaAssinanteRepresentanteList: AssinaturaAssinanteRepresentanteModel[]
  ) { }

  ngOnInit() {
   this.dataSource = this.assinaturaAssinanteRepresentanteList;
  }

  salvarRepresentantes(){
    if(this.atribuicoesEstaoValidas()){
      this.assinaturaService.atribuirRepresentantes(this.dataSource)
        .subscribe(p => {
          this.fecharModal(true);
        });
      }
  }

  atribuicoesEstaoValidas(): boolean {
    const errorList: string[] = [];

    this.dataSource.forEach(function(item){
      if(item.usuarioAdAssinanteGuid === item.usuarioAdRepresentanteGuid){
        errorList.push(`O representante de ${item.usuarioAdAssinate} deve ser diferente dele mesmo.`)
      }
    });

    if(errorList.length > 0){
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }

  fecharModal(sucesso){
    this.dialogRef.close(sucesso);
  }
}
