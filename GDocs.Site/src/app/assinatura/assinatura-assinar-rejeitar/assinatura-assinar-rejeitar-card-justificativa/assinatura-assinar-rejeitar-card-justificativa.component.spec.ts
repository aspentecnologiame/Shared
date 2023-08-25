import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaAssinarRejeitarCardJustificativaComponent } from './assinatura-assinar-rejeitar-card-justificativa.component';

describe('AssinaturaAssinarRejeitarCardJustificativaComponent', () => {
  let component: AssinaturaAssinarRejeitarCardJustificativaComponent;
  let fixture: ComponentFixture<AssinaturaAssinarRejeitarCardJustificativaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaAssinarRejeitarCardJustificativaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinarRejeitarCardJustificativaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
