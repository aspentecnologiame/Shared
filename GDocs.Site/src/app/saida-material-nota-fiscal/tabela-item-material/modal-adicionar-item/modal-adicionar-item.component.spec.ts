import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalAdicionarItemComponent } from './modal-adicionar-item.component';

describe('ModalAdicionarItemComponent', () => {
  let component: ModalAdicionarItemComponent;
  let fixture: ComponentFixture<ModalAdicionarItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalAdicionarItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalAdicionarItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
