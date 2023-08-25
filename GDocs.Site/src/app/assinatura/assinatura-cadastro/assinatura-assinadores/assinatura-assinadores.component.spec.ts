import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaAssinadoresComponent } from './assinatura-assinadores.component';

describe('AssinaturaAssinadoresComponent', () => {
  let component: AssinaturaAssinadoresComponent;
  let fixture: ComponentFixture<AssinaturaAssinadoresComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaAssinadoresComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaAssinadoresComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
