import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaGerenciamentoTabelaComponent } from './tabela.component';

describe('AssinaturaGerenciamentoTabelaComponent', () => {
  let component: AssinaturaGerenciamentoTabelaComponent;
  let fixture: ComponentFixture<AssinaturaGerenciamentoTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaGerenciamentoTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaGerenciamentoTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
