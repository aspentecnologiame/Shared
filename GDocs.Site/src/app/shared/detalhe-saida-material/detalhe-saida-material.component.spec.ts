import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DetalheSaidaMaterialComponent } from './detalhe-saida-material.component';

describe('DetalheSaidaMaterialComponent', () => {
  let component: DetalheSaidaMaterialComponent;
  let fixture: ComponentFixture<DetalheSaidaMaterialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DetalheSaidaMaterialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DetalheSaidaMaterialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
