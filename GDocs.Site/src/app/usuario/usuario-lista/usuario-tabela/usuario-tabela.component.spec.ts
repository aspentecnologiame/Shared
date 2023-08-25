import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuarioTabelaComponent } from './usuario-tabela.component';

describe('UsuarioTabelaComponent', () => {
  let component: UsuarioTabelaComponent;
  let fixture: ComponentFixture<UsuarioTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsuarioTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsuarioTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
