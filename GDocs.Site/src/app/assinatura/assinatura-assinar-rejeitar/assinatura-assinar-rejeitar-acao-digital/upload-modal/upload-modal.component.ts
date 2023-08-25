import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { Subject } from 'rxjs';
import { ExibicaoDeAlertaService, TipoAlertaEnum } from 'src/app/core';
import { UploadResponseModel } from 'src/app/shared/models/upload-response.model';

@Component({
  selector: 'app-upload-modal',
  templateUrl: './upload-modal.component.html',
  styleUrls: ['./upload-modal.component.scss']
})
export class UploadDocumentoAssinadoDigitalmenteModalComponent {
  listaDocumentosUpload: UploadResponseModel[] = [];
  listarDocumentosChangingValue: Subject<UploadResponseModel[]> = new Subject();
  habilitarPreview = false;

  constructor(
    public dialogRef: MatDialogRef<UploadDocumentoAssinadoDigitalmenteModalComponent>,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService
  ) { }

  uploadCompletoAdicionarNaListaDeDocumentosPreview(uploadResponseModel: UploadResponseModel): void {
    this.listaDocumentosUpload.push(uploadResponseModel);
    this.listarDocumentosChangingValue.next([this.listaDocumentosUpload[this.listaDocumentosUpload.length-1]]);

    this.alertaUploadSucesso(uploadResponseModel);
  }

  alertaUploadSucesso(uploadResponseModel){
    this.exibicaoDeAlertaService.exibirMensagem({
      titulo: "Sucesso!",
      mensagem: `Upload realizado com sucesso. Clique em OK para realizar etapa de confirmação de assinatura digital.`,
      tipo: TipoAlertaEnum.Sucesso,
      textoBotaoOk: "OK"
    }).then(_ => {
      this.fecharModal(uploadResponseModel);
    });
  }

  fecharModal(uploadResponse) {
    this.dialogRef.close(uploadResponse);
  }
}
