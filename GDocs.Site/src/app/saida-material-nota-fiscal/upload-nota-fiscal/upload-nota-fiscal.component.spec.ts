import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadNotaFiscalComponent } from './upload-nota-fiscal.component';

describe('UploadNotaFiscalComponent', () => {
  let component: UploadNotaFiscalComponent;
  let fixture: ComponentFixture<UploadNotaFiscalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadNotaFiscalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadNotaFiscalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
