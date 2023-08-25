import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CadastroComponent as CadastroMaterialNFComponent } from './cadastro.component';

describe('CadastroComponent', () => {
  let component: CadastroMaterialNFComponent;
  let fixture: ComponentFixture<CadastroMaterialNFComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CadastroMaterialNFComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CadastroMaterialNFComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
