import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaAssinarRejeitarListaComponent } from './assinatura-assinar-rejeitar-lista.component';

describe('AssinaturaAssinarRejeitarListaComponent', () => {
  let component: AssinaturaAssinarRejeitarListaComponent;
  let fixture: ComponentFixture<AssinaturaAssinarRejeitarListaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaAssinarRejeitarListaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinarRejeitarListaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
