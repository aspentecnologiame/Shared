import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaGerenciamentoRepresentanteTabelaComponent } from './representante-tabela.component';

describe('AssinaturaGerenciamentoRepresentanteTabelaComponent', () => {
  let component: AssinaturaGerenciamentoRepresentanteTabelaComponent;
  let fixture: ComponentFixture<AssinaturaGerenciamentoRepresentanteTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaGerenciamentoRepresentanteTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaGerenciamentoRepresentanteTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
