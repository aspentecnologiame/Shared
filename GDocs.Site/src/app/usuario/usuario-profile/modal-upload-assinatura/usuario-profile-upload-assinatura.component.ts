import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { ComponentFormBase, ExibicaoDeAlertaService } from '../../../core';
import { UploadResponseModel } from '../../../shared/models/upload-response.model';
import { AssinaturaBase64UsuarioModel } from '../../models';
import { UsuarioService } from '../../usuario.service';

@Component({
  selector: 'app-usuario-profile-upload-assinatura',
  templateUrl: './usuario-profile-upload-assinatura.component.html',
  styleUrls: ['./usuario-profile-upload-assinatura.component.scss']
})
export class UsuarioProfileUploadAssinaturaComponent extends ComponentFormBase implements OnInit {
  uploadResponseModel: UploadResponseModel = new UploadResponseModel();
  imageUrl = ''
  constructor(
    private readonly usuarioService: UsuarioService,
    public dialogRef: MatDialogRef<UsuarioProfileUploadAssinaturaComponent>,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
  ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
  }

  uploadCompletoAdicionarNaListaDeDocumentosPreview(uploadResponseModel: UploadResponseModel): void {
    this.uploadResponseModel = uploadResponseModel;
    this.imageUrl = 'data:image/png;base64,' + this.uploadResponseModel.arquivoBinario;
  }

  save(): void {
    if (this.uploadResponseModel.arquivoBinario === '') {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'É necessário fazer o upload de um arquivo.');
      return;
    }

    const assinaturaBase64UsuarioModel = new AssinaturaBase64UsuarioModel(this.uploadResponseModel.arquivoBinario);

    this.usuarioService.gravarAssinaturaBase64Usuario(assinaturaBase64UsuarioModel)
      .subscribe(
        resposta => {
          this.exibicaoDeAlertaService
            .exibirMensagemSucesso('Sucesso!', 'Assinatura gravada com sucesso !')
            .then(() => {
              this.dialogRef.close({retorno: true});
            });
        },
        error => {

          this.exibicaoDeAlertaService
            .exibirMensagemErro('Erro!', 'Ocorreu um erro ao gravar a assinatura !')
            .then(() => {
              this.dialogRef.close();
            });
        }
      );
  }

  fecharModal() {
    if(this.imageUrl !== '') {
      this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao('Deseja salvar o upload da assinatura ?')
      .then(resposta => {
        if (resposta.value) {
          this.save();
        } else {
          this.dialogRef.close();
        }
      });
    } else {
      this.dialogRef.close();
    }
  }
}
