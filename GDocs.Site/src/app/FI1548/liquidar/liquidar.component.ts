import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { TipoAlertaEnum } from 'src/app/core';
import { ComponentFormBase } from 'src/app/core/component-form-base';
import { ExibicaoDeAlertaService } from 'src/app/core/services/exibicao-de-alerta/exibicao-de-alerta.service';
import { FormValidationHelper } from 'src/app/helpers/form-validation.helper';
import { UploadResponseModel } from 'src/app/shared/models/upload-response.model';
import { Fi1548Service } from '../FI1548.service';
import { DocumentoModel } from '../shared/models/documento.model';

@Component({
  selector: 'app-liquidar',
  templateUrl: './liquidar.component.html',
  styleUrls: ['./liquidar.component.scss']
})
export class LiquidarComponent extends ComponentFormBase implements OnInit {
numero: number;
formDocumento: FormGroup;
formValidationHelper: FormValidationHelper;
listaDocumentosUpload: UploadResponseModel[] = [];
listarDocumentosChangingValue: Subject<UploadResponseModel[]> = new Subject();

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
    private readonly fi1548Service: Fi1548Service
    ) {
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.numero = parseInt(this.route.snapshot.paramMap.get('numero'));
    this.formInit();
  }

  private formInit(): void {
     this.formDocumento = this.formBuilder.group({
      numero: new FormControl(this.numero, [Validators.required]),
      nomeArquivo: new FormControl(null, [Validators.required])
     });

     this.formValidationHelper = new FormValidationHelper({
      documentoId: 'Número do documento',
      nomeArquivo: 'Documento PDF'
     });
  }

  liquidarDocumento(){
    if(this.validarLiquidacaoDocumento()){
      this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao(
        `<p>Tem certeza que deseja liquidar o documento ${this.formDocumento.controls['numero'].value}?</p>
         <p><strong>Observação: As alterações realizadas após à liquidação do documento não poderão serem revertidas!</strong></p>`)
        .then(resposta => {
          if (resposta.value) {
            var documentoLiquidar = new DocumentoModel();
            documentoLiquidar.numero =  this.formDocumento.controls['numero'].value;
            documentoLiquidar.nomeSalvo = this.listaDocumentosUpload[this.listaDocumentosUpload.length-1].nomeSalvo;

            this.fi1548Service.liquidarDocumento(documentoLiquidar)
               .subscribe(l => {
                  this.exibicaoDeAlertaService.exibirMensagem({
                    titulo: "Sucesso!",
                    mensagem: `Documento ${documentoLiquidar.numero} foi liquidado com sucesso!`,
                    tipo: TipoAlertaEnum.Sucesso,
                    textoBotaoOk: "OK"
                  }).then(_ => {
                    this.navegarParaConsulta();
                  });
               });
          }
        });
    }
  }

  private validarLiquidacaoDocumento(): boolean{
    const errorList: Array<string> = this.formValidationHelper.getFormValidationErrorsMessages(this.formDocumento.controls);

    if (errorList.length > 0) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', errorList.join('<br />'));
      return false;
    }

    return true;
  }

  navegarParaConsulta(){
    this.router.navigate(['fi1548/consultar']);
  }

  uploadCompletoAdicionarNaListaDeDocumentosPreview(uploadResponseModel: UploadResponseModel): void {
    this.formDocumento.controls['nomeArquivo'].setValue(uploadResponseModel.nomeSalvo);
    this.listaDocumentosUpload.push(uploadResponseModel);
    this.listarDocumentosChangingValue.next([this.listaDocumentosUpload[this.listaDocumentosUpload.length-1]]);
  }
}
