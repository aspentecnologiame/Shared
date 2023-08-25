import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AssinaturaAssinarRejeitarAcaoComponent } from './assinatura-assinar-rejeitar-acao.component';

describe('AssinaturaAssinarRejeitarAcaoComponent', () => {
  let component: AssinaturaAssinarRejeitarAcaoComponent;
  let fixture: ComponentFixture<AssinaturaAssinarRejeitarAcaoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AssinaturaAssinarRejeitarAcaoComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinarRejeitarAcaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
