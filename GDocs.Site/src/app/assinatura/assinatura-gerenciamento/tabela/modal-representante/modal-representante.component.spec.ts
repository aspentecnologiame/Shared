import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaGerenciamentoRepresentanteComponent } from './modal-representante.component';

describe('AssinaturaGerenciamentoRepresentanteComponent', () => {
  let component: AssinaturaGerenciamentoRepresentanteComponent;
  let fixture: ComponentFixture<AssinaturaGerenciamentoRepresentanteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaGerenciamentoRepresentanteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaGerenciamentoRepresentanteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
