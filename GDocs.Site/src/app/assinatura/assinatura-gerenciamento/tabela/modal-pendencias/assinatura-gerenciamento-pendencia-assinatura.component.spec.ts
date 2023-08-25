import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaGerenciamentoPendenciaAssinaturaComponent } from './assinatura-gerenciamento-pendencia-assinatura.component';

describe('AssinaturaGerenciamentoPendenciaAssinaturaComponent', () => {
  let component: AssinaturaGerenciamentoPendenciaAssinaturaComponent;
  let fixture: ComponentFixture<AssinaturaGerenciamentoPendenciaAssinaturaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaGerenciamentoPendenciaAssinaturaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaGerenciamentoPendenciaAssinaturaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
