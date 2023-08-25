import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsuarioProfileUploadAssinaturaComponent } from './usuario-profile-upload-assinatura.component';

describe('UsuarioProfileUploadAssinaturaComponent', () => {
  let component: UsuarioProfileUploadAssinaturaComponent;
  let fixture: ComponentFixture<UsuarioProfileUploadAssinaturaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsuarioProfileUploadAssinaturaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsuarioProfileUploadAssinaturaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
