import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { concat, Observable, of, Subject } from 'rxjs';
import { catchError, debounceTime, distinctUntilChanged, map, switchMap, tap } from 'rxjs/operators';
import { AssinaturaService } from 'src/app/assinatura/assinatura.service';
import { AssinaturaAssinanteRepresentanteModel } from 'src/app/assinatura/models/assinatura-assinante-representante.model';

@Component({
  selector: 'app-representante-auto-complete',
  templateUrl: './representante-auto-complete.component.html',
  styleUrls: ['./representante-auto-complete.component.scss']
})
export class RepresentanteAutoCompleteComponent implements OnInit {
  @Input() assinaturaAssinanteRepresentante: AssinaturaAssinanteRepresentanteModel;
  @Output() assinaturaAssinanteRepresentanteChange = new EventEmitter<AssinaturaAssinanteRepresentanteModel>();
  obsListaRepresentante$: Observable<AssinaturaAssinanteRepresentanteModel[]>;
  listaRepresentanteinput$ = new Subject<string>();
  listaRepresentanteLoading = false;

  constructor(
    private readonly assinaturaService: AssinaturaService
  ) { }

  ngOnInit() {
    this.carregarRepresentantes();
  }

  private carregarRepresentantes() {
    const arrayRepresentante: AssinaturaAssinanteRepresentanteModel[] = [];

    if(this.assinaturaAssinanteRepresentante.usuarioAdRepresentante){
      arrayRepresentante.push(this.assinaturaAssinanteRepresentante)
    }

    this.obsListaRepresentante$ = concat(
      of([...arrayRepresentante]),
      this.listaRepresentanteinput$.pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => this.listaRepresentanteLoading = true),
        switchMap(term => {
          if (term === undefined || term === null || term.trim().length < 3) {
            this.listaRepresentanteLoading = false;
            return of([]);
          }

          return this.assinaturaService.listarUsuariosAdParaAssinaturaPorNome(term.trim())
          .pipe(
            map(event => {
              return event.map(n => {
                const passo = new AssinaturaAssinanteRepresentanteModel();
                passo.passoId = this.assinaturaAssinanteRepresentante.passoId;
                passo.usuarioAdRepresentanteGuid = n.guid;
                passo.usuarioAdRepresentante = n.nome;
                return passo;
              });
            }),
            catchError(() => of([])),
            tap(() => this.listaRepresentanteLoading = false)
          );
        })
      )
    );
  }

  onChange(event: AssinaturaAssinanteRepresentanteModel) {
    if(event) {
        this.assinaturaAssinanteRepresentanteChange.emit(event);
    } else {
        this.assinaturaAssinanteRepresentanteChange.emit(
          Object.assign({}, this.assinaturaAssinanteRepresentante, { usuarioAdRepresentanteGuid: null } )
        );
    }
  }
}
