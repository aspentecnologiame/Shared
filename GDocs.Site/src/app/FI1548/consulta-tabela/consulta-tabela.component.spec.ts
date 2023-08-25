import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConsultaTabelaComponent } from './consulta-tabela.component';

describe('ConsultaTabelaComponent', () => {
  let component: ConsultaTabelaComponent;
  let fixture: ComponentFixture<ConsultaTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConsultaTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConsultaTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
