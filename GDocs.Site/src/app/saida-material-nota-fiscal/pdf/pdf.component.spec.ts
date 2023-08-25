import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PdfComponent as PdfMaterialNf } from './pdf.component';

describe('PdfComponent', () => {
  let component: PdfMaterialNf;
  let fixture: ComponentFixture<PdfMaterialNf>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PdfMaterialNf ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PdfMaterialNf);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
