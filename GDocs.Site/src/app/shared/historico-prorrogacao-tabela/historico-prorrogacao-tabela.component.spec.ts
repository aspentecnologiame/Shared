import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HistoricoProrrogacaoTabelaComponent } from './historico-prorrogacao-tabela.component';

describe('HistoricoProrrogacaoTabelaComponent', () => {
  let component: HistoricoProrrogacaoTabelaComponent;
  let fixture: ComponentFixture<HistoricoProrrogacaoTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HistoricoProrrogacaoTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoricoProrrogacaoTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
