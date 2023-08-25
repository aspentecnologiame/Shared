import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaGerenciamentoComponent } from './assinatura-gerenciamento.component';

describe('AssinaturaGerenciamentoComponent', () => {
  let component: AssinaturaGerenciamentoComponent;
  let fixture: ComponentFixture<AssinaturaGerenciamentoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaGerenciamentoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaGerenciamentoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
