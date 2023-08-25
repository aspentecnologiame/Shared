import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadDocumentoAssinadoDigitalmenteModalComponent } from './upload-modal.component';

describe('UploadDocumentoAssinadoDigitalmenteModalComponent', () => {
  let component: UploadDocumentoAssinadoDigitalmenteModalComponent;
  let fixture: ComponentFixture<UploadDocumentoAssinadoDigitalmenteModalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadDocumentoAssinadoDigitalmenteModalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadDocumentoAssinadoDigitalmenteModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
