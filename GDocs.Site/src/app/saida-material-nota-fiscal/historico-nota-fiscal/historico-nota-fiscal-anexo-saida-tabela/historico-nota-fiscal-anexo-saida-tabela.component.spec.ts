import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HistoricoNotaFiscalAnexoSaidaTabelaComponent } from './historico-nota-fiscal-anexo-saida-tabela.component';

describe('HistoricoNotaFiscalAnexoSaidaTabelaComponent', () => {
  let component: HistoricoNotaFiscalAnexoSaidaTabelaComponent;
  let fixture: ComponentFixture<HistoricoNotaFiscalAnexoSaidaTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HistoricoNotaFiscalAnexoSaidaTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoricoNotaFiscalAnexoSaidaTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
