import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaCienciaAcaoCardJustificativaComponent } from './assinatura-ciencia-acao-card-justificativa.component';

describe('AssinaturaCienciaAcaoCardJustificativaComponent', () => {
  let component: AssinaturaCienciaAcaoCardJustificativaComponent;
  let fixture: ComponentFixture<AssinaturaCienciaAcaoCardJustificativaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaCienciaAcaoCardJustificativaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaCienciaAcaoCardJustificativaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
