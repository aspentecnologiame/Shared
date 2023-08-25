import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JustificativaCienciaRejeitadaComponent } from './justificativa-ciencia-rejeitada.component';

describe('JustificativaCienciaRejeitadaComponent', () => {
  let component: JustificativaCienciaRejeitadaComponent;
  let fixture: ComponentFixture<JustificativaCienciaRejeitadaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JustificativaCienciaRejeitadaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JustificativaCienciaRejeitadaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
