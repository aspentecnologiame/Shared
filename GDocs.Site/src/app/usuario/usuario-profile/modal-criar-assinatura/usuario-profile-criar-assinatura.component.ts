import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { fromEvent } from 'rxjs';
import { pairwise, switchMap, takeUntil } from 'rxjs/operators';
import { UsuarioService } from '../../usuario.service';
import { ComponentFormBase, ExibicaoDeAlertaService } from '../../../core';
import { AssinaturaBase64UsuarioModel } from '../../models';

@Component({
  selector: 'app-usuario-profile-criar-assinatura',
  templateUrl: './usuario-profile-criar-assinatura.component.html',
  styleUrls: ['./usuario-profile-criar-assinatura.component.scss'],
  providers: [UsuarioService]
})
export class UsuarioProfileCriarAssinaturaComponent extends ComponentFormBase implements OnInit, AfterViewInit {
  constructor(
    public dialogRef: MatDialogRef<UsuarioProfileCriarAssinaturaComponent>,
    private readonly usuarioService: UsuarioService,
    private readonly exibicaoDeAlertaService: ExibicaoDeAlertaService,
  ) {
    super();
  }

  @ViewChild('canvas') public canvas: ElementRef;
  private cx: CanvasRenderingContext2D;

  ngOnInit() {
    super.ngOnInit();
  }

  public ngAfterViewInit() {
    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    this.cx = canvasEl.getContext('2d');

    canvasEl.width = 765;
    canvasEl.height = 205;

    this.cx.lineWidth = 3;
    this.cx.lineCap = 'round';
    this.cx.strokeStyle = '#000';

    this.captureEvents(canvasEl);
  }

  private captureEvents(canvasEl: HTMLCanvasElement) {
    // this will capture all mousedown events from the canvas element
    fromEvent(canvasEl, 'mousedown')
      .pipe(
        switchMap((e) => {
          // after a mouse down, we'll record all mouse moves
          return fromEvent(canvasEl, 'mousemove')
            .pipe(
              // we'll stop (and unsubscribe) once the user releases the mouse
              // this will trigger a 'mouseup' event
              takeUntil(fromEvent(canvasEl, 'mouseup')),
              // we'll also stop (and unsubscribe) once the mouse leaves the canvas (mouseleave event)
              takeUntil(fromEvent(canvasEl, 'mouseleave')),
              // pairwise lets us get the previous value to draw a line from
              // the previous point to the current point
              pairwise()
            )
        })
      )
      .subscribe((res: [MouseEvent, MouseEvent]) => {
        const rect = canvasEl.getBoundingClientRect();

        // previous and current position with the offset
        const prevPos = {
          x: res[0].clientX - rect.left,
          y: res[0].clientY - rect.top
        };

        const currentPos = {
          x: res[1].clientX - rect.left,
          y: res[1].clientY - rect.top
        };

        // this method we'll implement soon to do the actual drawing
        this.drawOnCanvas(prevPos, currentPos);
      });
  }

  private drawOnCanvas(prevPos: { x: number, y: number }, currentPos: { x: number, y: number }) {
    if (!this.cx) {
      return;
    }

    this.cx.beginPath();

    if (prevPos) {
      this.cx.moveTo(prevPos.x, prevPos.y); // from
      this.cx.lineTo(currentPos.x, currentPos.y);
      this.cx.stroke();
    }
  }

  public reset(): void {
    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    this.cx = canvasEl.getContext('2d');
    this.cx.clearRect(0, 0, canvasEl.width, canvasEl.height);
  }

  public save(): void {
    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    const assinaturaCanvas = canvasEl.toDataURL('image/png').split(",");
    const assinaturaBase64 = assinaturaCanvas[1];
    const assinaturaEmBranco = this.verificarAssinaturaEmBranco();

    if(assinaturaEmBranco) {
      this.exibicaoDeAlertaService.exibirMensagemAviso('Atenção!', 'É necessário criar uma assinatura.');
      return;
    }

    const assinaturaBase64UsuarioModel = new AssinaturaBase64UsuarioModel(assinaturaBase64);

    this.usuarioService.gravarAssinaturaBase64Usuario(assinaturaBase64UsuarioModel)
    .subscribe(
      resposta => {
        this.reset();
        this.exibicaoDeAlertaService
        .exibirMensagemSucesso('Sucesso!', 'Assinatura gravada com sucesso !')
        .then(() => {
          this.dialogRef.close({retorno: true});
        });
      },
      error => {
        this.reset();
        this.exibicaoDeAlertaService
        .exibirMensagemErro('Erro!', 'Ocorreu um erro ao gravar a assinatura !')
        .then(() => {
          this.dialogRef.close();
        });
      }
    );
  }

  fecharModal() {
    const assinaturaEmBranco = this.verificarAssinaturaEmBranco();

    if(!assinaturaEmBranco) {
      this.exibicaoDeAlertaService.exibirMensagemInterrogacaoSimNao('Deseja salvar o cadastro da assinatura ?')
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

  private verificarAssinaturaEmBranco(): boolean {

    const canvasEl: HTMLCanvasElement = this.canvas.nativeElement;
    this.cx = canvasEl.getContext('2d');

    const pixelBuffer = new Uint32Array(
      this.cx.getImageData(0, 0, canvasEl.width, canvasEl.height).data.buffer
    );

    return !pixelBuffer.some(color => color !== 0);
  }
}
