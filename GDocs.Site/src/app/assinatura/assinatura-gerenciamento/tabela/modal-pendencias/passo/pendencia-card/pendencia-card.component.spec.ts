import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PendenciaCardComponent } from './pendencia-card.component';

describe('PendenciaCardComponent', () => {
  let component: PendenciaCardComponent;
  let fixture: ComponentFixture<PendenciaCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PendenciaCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PendenciaCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
