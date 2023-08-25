import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsultaTabelaComponent as ConsultaTabelaMaterialNFComponent } from './consulta-tabela.component';

describe('ConsultaTabelaComponent', () => {
  let component: ConsultaTabelaMaterialNFComponent;
  let fixture: ComponentFixture<ConsultaTabelaMaterialNFComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsultaTabelaMaterialNFComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsultaTabelaMaterialNFComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
