import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InformacaoSaidaMaterialComponent } from './informacao-saida-material.component';

describe('InformacaoSaidaMaterialComponent', () => {
  let component: InformacaoSaidaMaterialComponent;
  let fixture: ComponentFixture<InformacaoSaidaMaterialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InformacaoSaidaMaterialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformacaoSaidaMaterialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
