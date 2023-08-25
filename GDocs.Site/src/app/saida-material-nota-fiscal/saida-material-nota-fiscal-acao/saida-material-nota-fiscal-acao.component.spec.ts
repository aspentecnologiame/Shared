import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaidaMaterialNotaFiscalAcaoComponent } from './saida-material-nota-fiscal-acao.component';

describe('SaidaMaterialNotaFiscalAcaoComponent', () => {
  let component: SaidaMaterialNotaFiscalAcaoComponent;
  let fixture: ComponentFixture<SaidaMaterialNotaFiscalAcaoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaidaMaterialNotaFiscalAcaoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaidaMaterialNotaFiscalAcaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
