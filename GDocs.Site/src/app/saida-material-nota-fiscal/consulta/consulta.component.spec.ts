import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsultaComponent as ConsultaMaterialNFComponent } from './consulta.component';

describe('ConsultaComponent', () => {
  let component: ConsultaMaterialNFComponent;
  let fixture: ComponentFixture<ConsultaMaterialNFComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsultaMaterialNFComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsultaMaterialNFComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
