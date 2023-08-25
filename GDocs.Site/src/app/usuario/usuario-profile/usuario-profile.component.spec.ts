import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuarioProfileComponent } from './usuario-profile.component';

describe('UsuarioProfileComponent', () => {
  let component: UsuarioProfileComponent;
  let fixture: ComponentFixture<UsuarioProfileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsuarioProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsuarioProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
