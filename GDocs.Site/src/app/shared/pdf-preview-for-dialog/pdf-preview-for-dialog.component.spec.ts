import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { PdfPreviewForDialogComponent } from './pdf-preview-for-dialog.component';


describe('DocumentoDetalheComponent', () => {
  let component: PdfPreviewForDialogComponent;
  let fixture: ComponentFixture<PdfPreviewForDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [PdfPreviewForDialogComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PdfPreviewForDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
