import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RepresentanteAutoCompleteComponent } from './representante-auto-complete.component';

describe('RepresentanteAutoCompleteComponent', () => {
  let component: RepresentanteAutoCompleteComponent;
  let fixture: ComponentFixture<RepresentanteAutoCompleteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RepresentanteAutoCompleteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RepresentanteAutoCompleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
