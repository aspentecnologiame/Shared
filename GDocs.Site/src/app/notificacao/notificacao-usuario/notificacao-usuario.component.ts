import { Component, OnInit, ViewChild, Input, ElementRef, Renderer2 } from '@angular/core';
import { ComponentFormBase } from './../../core/component-form-base';
import { NotificacaoRelatorioModel } from '../models/notificacao-relatorio.model';
import { NotificacaoService } from '../notificacao.service';
import { AutenticacaoService, UserDataModel } from 'src/app/autenticacao';
import { MatTableDataSource, MatPaginator, MatDialog } from '@angular/material';
import { BehaviorSubject } from 'rxjs';
import { PdfComponent } from '../pdf/pdf.component';

@Component({
  selector: 'app-notificacao-usuario',
  templateUrl: './notificacao-usuario.component.html'
})
export class NotificacaoUsuarioComponent extends ComponentFormBase implements OnInit {

  usuario: UserDataModel;
  existemRegistrosASeremExibidos: boolean = false;
  @Input() dataSource: NotificacaoRelatorioModel[] = [];

  displayedColumns: string[] = ['nome', 'dataCriacao', 'acoes'];
  matDataSource: MatTableDataSource<NotificacaoRelatorioModel>;

  @ViewChild(MatPaginator) paginator: MatPaginator;

  @ViewChild('msgRelatoriosNaoLidos') msgRelatoriosNaoLidos: ElementRef;

  dataChange = new BehaviorSubject<NotificacaoRelatorioModel[]>([]);
  liNotificacaoUsuario: HTMLElement;

  constructor(
    private readonly notificacaoService: NotificacaoService,
    private readonly autenticacaoService: AutenticacaoService,
    public dialog: MatDialog,
    private renderer: Renderer2
  ) { 
    super();
  }

  ngOnInit() {
    super.ngOnInit();
    this.usuario = this.autenticacaoService.obterUsuarioLogado() || new UserDataModel();
    this.obterNotificacoesNaoLidas();
  }

  exibirDetalhesDocumento(notificacaoUsuario: any): void {
    const dialogRef = this.dialog.open(PdfComponent, {
      width: '920px',
      data: notificacaoUsuario,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.liNotificacaoUsuario = document.querySelector('#liNotificacaoUsuario');
      if (result) {
        this.liNotificacaoUsuario.style.display = 'none';
        this.ngOnInit();
      }
    });
  }

  private obterNotificacoesNaoLidas() : void {
    this.notificacaoService.obterNaoLidasPorIdUsuario(this.usuario.id).subscribe(resposta => {
      this.dataSource = resposta;
      this.existemRegistrosASeremExibidos = resposta.length > 0;
      if (!this.existemRegistrosASeremExibidos) {
        this.renderer.setStyle(this.msgRelatoriosNaoLidos.nativeElement, 'color', '#000');
      }
      this.configureDataSource(this.dataSource);
      },
      error => {
        console.error('Erro no método carregarUsuarios.', error);
      }
    );
  }

  private configureDataSource(aplicativos: NotificacaoRelatorioModel[]): void {
    this.matDataSource = new MatTableDataSource<NotificacaoRelatorioModel>(aplicativos);
    this.paginator.length = this.matDataSource.data.length;
    this.paginator.pageSizeOptions = [7, 10, 20, 30, 40, 50, 100];
    this.matDataSource.paginator = this.paginator;  
  }

}
