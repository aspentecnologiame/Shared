import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuarioProfileCriarAssinaturaComponent } from './usuario-profile-criar-assinatura.component';

describe('UsuarioProfileCriarAssinaturaComponent', () => {
  let component: UsuarioProfileCriarAssinaturaComponent;
  let fixture: ComponentFixture<UsuarioProfileCriarAssinaturaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsuarioProfileCriarAssinaturaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsuarioProfileCriarAssinaturaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
