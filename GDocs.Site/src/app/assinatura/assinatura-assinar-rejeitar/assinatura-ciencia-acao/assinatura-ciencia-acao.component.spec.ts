import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaCienciaAcaoComponent } from './assinatura-ciencia-acao.component';

describe('AssinaturaCienciaAcaoComponent', () => {
  let component: AssinaturaCienciaAcaoComponent;
  let fixture: ComponentFixture<AssinaturaCienciaAcaoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaCienciaAcaoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaCienciaAcaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
