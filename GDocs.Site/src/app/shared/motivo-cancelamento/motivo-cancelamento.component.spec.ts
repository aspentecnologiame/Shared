import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MotivoCancelamentoComponent } from './motivo-cancelamento.component';

describe('JustificativaCienciaRejeitadaComponent', () => {
  let component: MotivoCancelamentoComponent;
  let fixture: ComponentFixture<MotivoCancelamentoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MotivoCancelamentoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MotivoCancelamentoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
