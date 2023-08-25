import { AfterViewInit, Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { Subject } from 'rxjs';
import { ComponentFormBase } from 'src/app/core';
import { SaidaMaterialNotaFiscalService } from '../saida-material-nota-fiscal.service';
import { SaidaMaterialArquivoModel } from '../shared/models/SaidaMaterialArquivoModel';
import { TipoAnexo } from './../shared/enums/TipoAnexo';

@Component({
  selector: 'app-pdf',
  templateUrl: './pdf.component.html',
  styleUrls: ['./pdf.component.scss']
})
export class PdfComponent  extends ComponentFormBase implements OnInit, AfterViewInit {
  @ViewChild('pdfView') pdfView;

  listarArquivosChangingValue: Subject<SaidaMaterialArquivoModel[]> = new Subject();
  listaArquivoSaidaMaterialUpload: SaidaMaterialArquivoModel[] = [];
  tipoAnexo:TipoAnexo;

  constructor(
    private readonly service: SaidaMaterialNotaFiscalService,
    public readonly dialogRef: MatDialogRef<PdfComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }
  ngAfterViewInit(): void {
    this.visualizarPdf();
  }

  ngOnInit() {
    super.ngOnInit();
  }

  private visualizarPdf(): void {
     this.listaArquivoSaidaMaterialUpload = this.data
     this.listarArquivosChangingValue.next(this.listaArquivoSaidaMaterialUpload);
  }

  DocumentoSelecionado(doc:any){
    this.tipoAnexo = doc.tipoNatureza;
  }

  MostraMensagemSaida():boolean{
      return this.tipoAnexo == TipoAnexo.Saida;
  }

  fechar(): void {
    this.dialogRef.close(true);
  }


}
