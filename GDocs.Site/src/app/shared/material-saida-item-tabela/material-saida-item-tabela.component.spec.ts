import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialSaidaItemTabelaComponent } from './material-saida-item-tabela.component';

describe('MaterialSaidaItemTabelaComponent', () => {
  let component: MaterialSaidaItemTabelaComponent;
  let fixture: ComponentFixture<MaterialSaidaItemTabelaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialSaidaItemTabelaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialSaidaItemTabelaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
