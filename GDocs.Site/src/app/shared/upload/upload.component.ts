import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { AppConfig } from '../../../app/app.config';
import { AssinaturaUsuarioAdModel } from '../../../app/assinatura/models';
import { ExibicaoDeAlertaService } from '../../../app/core/services/exibicao-de-alerta/exibicao-de-alerta.service';
import { IFeatureCategorias } from '../models/app-config-feature-categorias.model';
import { UploadRequestModel } from '../models/upload-request.model';
import { UploadResponseModel } from '../models/upload-response.model';
import { UploadService } from '../services/upload.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent implements OnInit {

  @ViewChild('file') file;

  @Output() uploadCompletoAdicionarNaListaDeDocumentosPreview: EventEmitter<UploadResponseModel> = new EventEmitter();
  @Input() fileUploadType = 'document';
  @Input() categoriaId: number;
  @Input() numeroDocumento: number;
  @Input() qtdeDocumentos: number;
  @Input() assinaturaDocumentoUsuario: AssinaturaUsuarioAdModel[];

  form: FormGroup;
  configCategorias: IFeatureCategorias[];
  documento: File;
  documentoSelecionado: boolean;
  enviandoDocumento: boolean;
  percentDone = 0;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly uploadService: UploadService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService
  ) { }

  ngOnInit() {
    this.formInit();
  }

  ngAfterViewInit() {
    this.file.nativeElement.accept = this.retornarExtensoesPermitidas(this.fileUploadType);
  }

  private formInit() {
    this.form = this.formBuilder.group({
      documento: new FormControl('')
    });
    this.configCategorias = AppConfig.settings.categorias;
  }

  fileAddedOnchange() {
    const files: { [key: string]: File } = this.file.nativeElement.files;

    for (const key in files) {
      if (!isNaN(parseInt(key, 0))) {
        this.documento = files[key];
        this.documentoSelecionado = false;
        let validador = this.retornarExtensoesPermitidas(this.fileUploadType);

        if (validador.toLowerCase().includes(this.documento.name.split('.').pop().toLowerCase())) {
          this.form.controls['documento'].setValue(this.documento.name);
          this.documentoSelecionado = true;
        } else {
          this.resetarFormUpload();
          this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!',
            `Não é permitido o upload do arquivo selecionado. Realize o upload de um arquivo com formato válido: ${validador}.`);
        }
      }
    }
  }

  enviarArquivo() {
    this.percentDone = 0;

    this.enviandoDocumento = true;

    if (this.fileUploadType === 'image') {

      const file = this.file.nativeElement.files[0];
      const reader = new FileReader();
      reader.readAsDataURL(file);

      reader.onload = () => {
        this.enviandoDocumento = true;
        this.percentDone = 100
        this.updateInfoFile(new UploadResponseModel(this.documento.name, '', reader.result.toString().split(',')[1], '', 0, true));
      };
    } else {

      this.carregarCategoriaId();
      this.uploadService.enviarDocumento(new UploadRequestModel(
        this.documento,
        this.categoriaId,
        this.carregarGuidAssinatura(),
        this.qtdeDocumentos,
        this.numeroDocumento)
      ).subscribe(
          result => {
            if (result) {
              if (result.enviando === true) {
                this.updateProgressBar(result.porcentagemEnviada);
              } else {
                this.updateInfoFile(result.assinaturaUploadResonse);
                this.enviandoDocumento = false;
                this.resetarFormUpload();
                this.documentoSelecionado = false;
              }
            }
          },
          (err) => {
            this.percentDone = 0;
            this.enviandoDocumento = false;
            this.resetarFormUpload();
            this.documentoSelecionado = false;
          });
    }
  }

  private carregarCategoriaId() {
    if (this.categoriaId === undefined || this.categoriaId === null) {
      this.categoriaId = 0;
    }
  }

  private carregarGuidAssinatura(): any[] {
   if (this.assinaturaDocumentoUsuario !== null && this.assinaturaDocumentoUsuario !== undefined) {
    return this.assinaturaDocumentoUsuario.map(item => item.guid.toString());
   }
    return [];
  }

  adicionarArquivo() {
    this.updateProgressBar(0);
    this.file.nativeElement.click();
  }

  private updateProgressBar(percentDone: number): void {
    this.percentDone = percentDone;
  }

  private resetarFormUpload() {
    this.documento = null;
    this.file.nativeElement.value = '';
    this.form.controls['documento'].setValue('');
  }

  private updateInfoFile(informacoesDocumentoEnviado: UploadResponseModel): void {
    this.uploadCompletoAdicionarNaListaDeDocumentosPreview.emit(informacoesDocumentoEnviado);
  }

  private retornarExtensoesPermitidas(fileType: string): string {
    if (fileType === 'image') {
      return AppConfig.settings.fileExtensionsAllowedForUploadImage;
    } else if (fileType === 'pdf') {
      return AppConfig.settings.fileExtensionsAllowedOnlyPdf;
    }

    const categoria = this.configCategorias.find(w => w.codigo === this.categoriaId);

    if (categoria === null ||
        categoria === undefined ||
        categoria.extensoesPermitidas.length === 0 ||
        this.qtdeDocumentos !== 0)
      return AppConfig.settings.fileExtensionsAllowedForUploadToConvertToPdf;
    else {
      return `.${categoria.extensoesPermitidas.replace('|',',.')}`
    }
  }
}
