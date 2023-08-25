import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialSaidaAcaoComponent } from './material-saida-acao.component';

describe('MaterialSaidaAcaoComponent', () => {
  let component: MaterialSaidaAcaoComponent;
  let fixture: ComponentFixture<MaterialSaidaAcaoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialSaidaAcaoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialSaidaAcaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
