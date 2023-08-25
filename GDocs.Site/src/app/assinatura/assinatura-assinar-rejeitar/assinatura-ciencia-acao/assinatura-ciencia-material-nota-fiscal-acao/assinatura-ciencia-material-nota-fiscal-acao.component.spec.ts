import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaCienciaMaterialNotaFiscalAcaoComponent } from './assinatura-ciencia-material-nota-fiscal-acao.component';

describe('AssinaturaCienciaMaterialNotaFiscalAcaoComponent', () => {
  let component: AssinaturaCienciaMaterialNotaFiscalAcaoComponent;
  let fixture: ComponentFixture<AssinaturaCienciaMaterialNotaFiscalAcaoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaCienciaMaterialNotaFiscalAcaoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaCienciaMaterialNotaFiscalAcaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
