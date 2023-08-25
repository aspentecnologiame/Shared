import { HttpEventType } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { GDocsApiEndpointService } from 'src/app/core/services/gdocs-api/gdocs-api-endpoint.service';
import { InformacoesDocumentoEnviadoUploadResponseModel } from '../models/info-documento-upload-response.model';
import { UploadRequestModel } from '../models/upload-request.model';
import { UploadResponseModel } from '../models/upload-response.model';

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  constructor(private readonly gDocsApiEndpointService: GDocsApiEndpointService) { }

  private readonly URLS = {
    upload: '/v1/upload'
  };

  enviarDocumento(requisicao: UploadRequestModel): Observable<InformacoesDocumentoEnviadoUploadResponseModel> {
    const formData = new FormData();
    formData.append('file', requisicao.file);
    formData.append('CategoriaId', requisicao.categoriaId.toString());
    for( let i = 0; i < requisicao.guidUsuarioAssinaturaDocumento.length; i++ ) {
      const keyPrefix = `ListaGuidUsuarioAssinaturaDocumento[${i.toString()}]`;
      formData.append( keyPrefix, requisicao.guidUsuarioAssinaturaDocumento[i].toString()); 
    }
    if (requisicao.qtdeDocumentos !== null && requisicao.qtdeDocumentos !== undefined) {
      formData.append('QtdeDocumentos', requisicao.qtdeDocumentos.toString());
    }
    if (requisicao.numeroDocumento !== null && requisicao.numeroDocumento !== undefined) {
      formData.append('NumeroDocumento', requisicao.numeroDocumento.toString());
    }

    return this.gDocsApiEndpointService.executarRequisicaoPost(this.URLS.upload, formData, { reportProgress: true })
      .pipe(
        map(event => {
          switch (event.type) {
            case HttpEventType.UploadProgress:
              return new InformacoesDocumentoEnviadoUploadResponseModel(true, Math.round(100 * event.loaded / event.total));

            case HttpEventType.Response:
              return new InformacoesDocumentoEnviadoUploadResponseModel(false, 100, Object.assign(new UploadResponseModel(), event.body));

            default:
              return undefined;
          }
        })
      );
  }
}
