import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { AssinaturaService } from 'src/app/assinatura/assinatura.service';
import { AssinaturaUploadResponseModel } from 'src/app/assinatura/models';
import { ExibicaoDeAlertaService } from 'src/app/core/services/exibicao-de-alerta';

@Component({
  selector: 'app-cadastro-upload',
  templateUrl: './cadastro-upload.component.html',
  styleUrls: ['./cadastro-upload.component.scss']
})
export class CadastroUploadComponent implements OnInit {
  processoAssinaturaDocumentoId: number;
  listaAssinaturaDocUploadChangingValue: Subject<AssinaturaUploadResponseModel[]> = new Subject();
  listaAssinaturaDocUpload: AssinaturaUploadResponseModel[] = [];

  @Input()
  set IncluiDataSource(listaAssinaturaUpload: AssinaturaUploadResponseModel[] ){
      if(listaAssinaturaUpload.length > 0){
         this.listaAssinaturaDocUpload = listaAssinaturaUpload
         this.AtualizaLista();
      }
  }


  constructor(private readonly router: Router,
    private readonly assinaturaService: AssinaturaService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService) { }

  ngOnInit() {
  }

  AtualizaLista(){
    this.processoAssinaturaDocumentoId = this.listaAssinaturaDocUpload.find(x => x.processoAssinaturaDocumentoId != null).processoAssinaturaDocumentoId;
    this.listaAssinaturaDocUploadChangingValue.next(this.listaAssinaturaDocUpload);
  }

  uploadCompletoAdicionarNaListaDeDocumentosPreview(assinaturaUploadResponseModel: AssinaturaUploadResponseModel): void {
    this.listaAssinaturaDocUpload.push(assinaturaUploadResponseModel);
    this.listaAssinaturaDocUploadChangingValue.next(this.listaAssinaturaDocUpload);
  }

  salvarArquivo(): void {
    this.assinaturaService.gravarArquivos(this.processoAssinaturaDocumentoId, this.listaAssinaturaDocUpload).subscribe(() => {
      this.exibicaoDeAlertaService
        .exibirMensagemSucesso('Sucesso!', 'Arquivo(s) adicionado(s) para aprovação com sucesso!')
        .then(() => this.router.navigate(['fi1548/consultar/']));
    });
  }

}
