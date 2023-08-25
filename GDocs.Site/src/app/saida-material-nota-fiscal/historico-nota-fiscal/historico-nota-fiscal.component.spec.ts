import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HistoricoNotaFiscalComponent } from './historico-nota-fiscal.component';

describe('HistoricoNotaFiscalComponent', () => {
  let component: HistoricoNotaFiscalComponent;
  let fixture: ComponentFixture<HistoricoNotaFiscalComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HistoricoNotaFiscalComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoricoNotaFiscalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
