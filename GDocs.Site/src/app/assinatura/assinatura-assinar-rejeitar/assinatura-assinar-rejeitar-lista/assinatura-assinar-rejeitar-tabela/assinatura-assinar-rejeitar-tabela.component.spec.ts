import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaAssinarRejeitarTabelaComponent } from './assinatura-assinar-rejeitar-tabela.component';

describe('AssinaturaAssinarRejeitarTabelaComponent', () => {
  let component: AssinaturaAssinarRejeitarTabelaComponent;
  let fixture: ComponentFixture<AssinaturaAssinarRejeitarTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaAssinarRejeitarTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinarRejeitarTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
