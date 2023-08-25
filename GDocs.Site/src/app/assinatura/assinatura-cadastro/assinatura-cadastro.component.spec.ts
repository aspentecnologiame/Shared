import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AssinaturaCadastroComponent } from './assinatura-cadastro.component';

describe('AssinaturaCadastroComponent', () => {
  let component: AssinaturaCadastroComponent;
  let fixture: ComponentFixture<AssinaturaCadastroComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AssinaturaCadastroComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaCadastroComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
