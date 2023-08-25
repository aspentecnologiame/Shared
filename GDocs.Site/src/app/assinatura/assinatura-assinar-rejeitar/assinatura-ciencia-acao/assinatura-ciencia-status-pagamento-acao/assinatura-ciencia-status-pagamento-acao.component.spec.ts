import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaCienciaStatusPagamentoAcaoComponent } from './assinatura-ciencia-status-pagamento-acao.component';

describe('AssinaturaCienciaStatusPagamentoAcaoComponent', () => {
  let component: AssinaturaCienciaStatusPagamentoAcaoComponent;
  let fixture: ComponentFixture<AssinaturaCienciaStatusPagamentoAcaoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaCienciaStatusPagamentoAcaoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaCienciaStatusPagamentoAcaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
