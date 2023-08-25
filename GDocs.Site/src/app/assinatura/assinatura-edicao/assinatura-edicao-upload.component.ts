import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { ComponentFormBase, ExibicaoDeAlertaService } from '../../core';
import { AssinaturaService } from '../assinatura.service';
import { AssinaturaStatusDocumento } from '../enums/assinatura-status-documento-enum';
import { AssinaturaUploadResponseModel } from '../models';

@Component({
  selector: 'app-assinatura-edicao-upload',
  templateUrl: './assinatura-edicao-upload.component.html',
  styleUrls: ['./assinatura-edicao-upload.component.scss']
})
export class AssinaturaEdicaoUploadComponent extends ComponentFormBase implements OnInit {
  processoAssinaturaDocumentoId: number;
  processoDocumentoEdicaoId:number;
  listaAssinaturaDocumentosUpload: AssinaturaUploadResponseModel[] = [];
  listarDocumentosChangingValue: Subject<AssinaturaUploadResponseModel[]> = new Subject();

  constructor(
    private readonly router: Router,
    private readonly activedRoute: ActivatedRoute,
    private readonly assinaturaService: AssinaturaService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,) {
    super();
  }

  ngOnInit() {
    const padid = this.activedRoute.snapshot.paramMap.get('padid');
    const docId = this.activedRoute.snapshot.paramMap.get('docid');

    if (padid !== null && padid !== undefined && padid !== '' && !isNaN(Number(padid.toString()))) {
      this.processoAssinaturaDocumentoId = Number(padid);
      this.popularListAssinaturaDocumentosUpload();
    }

    if (docId !== null && docId !== undefined && docId !== '' && !isNaN(Number(docId.toString()))) {
      this.processoDocumentoEdicaoId = Number(docId);
    }

    super.ngOnInit();
  }

  popularListAssinaturaDocumentosUpload(): void {
    this.assinaturaService.listarArquivosUploadPorPadId(this.processoAssinaturaDocumentoId).subscribe((listaArquivos) => {
      if (listaArquivos.length === 0 ||
        listaArquivos.filter(arquivos => arquivos.processoAssinaturaDocumentoStatusId !== AssinaturaStatusDocumento.Emconstrucao).length > 0) {
        this.exibicaoDeAlertaService
          .exibirMensagemAviso('Atenção!', 'Não é possível aprovar/rejeitar esse processo de aprovação, pois ele não se encontra no status "Em construção".')
          .then(() => {
            this.navegarParaConsultaAssinaturasPendentes()
          });
      }
      this.listaAssinaturaDocumentosUpload.push(...listaArquivos);
      this.listarDocumentosChangingValue.next(this.listaAssinaturaDocumentosUpload);
    });
  }

  uploadCompletoAdicionarNaListaDeDocumentosPreview(assinaturaUploadResponseModel: AssinaturaUploadResponseModel): void {
    this.listaAssinaturaDocumentosUpload.push(assinaturaUploadResponseModel);
    this.listarDocumentosChangingValue.next(this.listaAssinaturaDocumentosUpload);
  }

  salvar(): void {
    this.assinaturaService.gravarArquivos(this.processoAssinaturaDocumentoId, this.listaAssinaturaDocumentosUpload).subscribe(() => {
      this.exibicaoDeAlertaService
        .exibirMensagemSucesso('Sucesso!', 'Arquivo(s) adicionado(s) para aprovação com sucesso!')
        .then(() => this.navegarParaConsultaAssinaturasPendentes());
    });
  }


  Editar(): void {
   if(this.listaAssinaturaDocumentosUpload.some(doc => doc.categoria === 1))
     this.router.navigate(['fi1548/adicionar/'+this.processoDocumentoEdicaoId]);
   else
     this.router.navigate(['fi347/adicionar/'+this.processoDocumentoEdicaoId]);
  }

  navegarParaConsultaAssinaturasPendentes(): void {
    if(this.listaAssinaturaDocumentosUpload.some(doc => doc.categoria === 1)){
      this.router.navigate(['fi1548/consultar/']);
     }
     else{
      this.router.navigate(['fi347/consultar/']);
     }
  }

}
