import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaInformacoesComponent } from './assinatura-informacoes.component';

describe('AssinaturaInformacoesComponent', () => {
  let component: AssinaturaInformacoesComponent;
  let fixture: ComponentFixture<AssinaturaInformacoesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaInformacoesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaInformacoesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
