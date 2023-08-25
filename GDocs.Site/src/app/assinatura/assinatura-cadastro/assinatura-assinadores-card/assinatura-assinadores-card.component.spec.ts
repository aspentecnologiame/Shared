import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaAssinadoresCardComponent } from './assinatura-assinadores-card.component';

describe('AssinaturaAssinadoresCardComponent', () => {
  let component: AssinaturaAssinadoresCardComponent;
  let fixture: ComponentFixture<AssinaturaAssinadoresCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaAssinadoresCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinadoresCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
