import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialSaidaModalComponent } from './material-saida-modal.component';

describe('MaterialSaidaModalComponent', () => {
  let component: MaterialSaidaModalComponent;
  let fixture: ComponentFixture<MaterialSaidaModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialSaidaModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialSaidaModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
