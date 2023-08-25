import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LiquidarComponent } from './liquidar.component';

describe('LiquidarComponent', () => {
  let component: LiquidarComponent;
  let fixture: ComponentFixture<LiquidarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LiquidarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LiquidarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
