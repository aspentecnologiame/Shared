import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CadastroUploadComponent } from './cadastro-upload.component';

describe('CadastroUploadComponent', () => {
  let component: CadastroUploadComponent;
  let fixture: ComponentFixture<CadastroUploadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CadastroUploadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CadastroUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
