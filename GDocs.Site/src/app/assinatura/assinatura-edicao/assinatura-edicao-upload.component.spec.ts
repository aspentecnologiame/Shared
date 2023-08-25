import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AssinaturaEdicaoUploadComponent } from './assinatura-edicao-upload.component';


describe('AssinaturaEdicaoUploadComponent', () => {
  let component: AssinaturaEdicaoUploadComponent;
  let fixture: ComponentFixture<AssinaturaEdicaoUploadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [AssinaturaEdicaoUploadComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AssinaturaEdicaoUploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
