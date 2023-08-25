import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaAssinarRejeitarAcaoDigitalComponent } from './assinatura-assinar-rejeitar-acao-digital.component';

describe('AssinaturaAssinarRejeitarAcaoDigitalComponent', () => {
  let component: AssinaturaAssinarRejeitarAcaoDigitalComponent;
  let fixture: ComponentFixture<AssinaturaAssinarRejeitarAcaoDigitalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaAssinarRejeitarAcaoDigitalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinarRejeitarAcaoDigitalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
