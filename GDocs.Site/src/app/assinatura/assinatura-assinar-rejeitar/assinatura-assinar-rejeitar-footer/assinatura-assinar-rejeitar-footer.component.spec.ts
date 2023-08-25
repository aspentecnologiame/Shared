import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaAssinarRejeitarFooterComponent } from './assinatura-assinar-rejeitar-footer.component';

describe('AssinaturaAssinarRejeitarFooterComponent', () => {
  let component: AssinaturaAssinarRejeitarFooterComponent;
  let fixture: ComponentFixture<AssinaturaAssinarRejeitarFooterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaAssinarRejeitarFooterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinarRejeitarFooterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
