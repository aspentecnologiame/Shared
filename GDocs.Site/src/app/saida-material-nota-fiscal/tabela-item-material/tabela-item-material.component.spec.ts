import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TabelaItemMaterialComponent } from './tabela-item-material.component';

describe('TabelaItemMaterialComponent', () => {
  let component: TabelaItemMaterialComponent;
  let fixture: ComponentFixture<TabelaItemMaterialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TabelaItemMaterialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TabelaItemMaterialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
