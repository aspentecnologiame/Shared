import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NoRecordsFoundMessageComponent } from './no-records-found-message.component';

describe('NoRecordsFoundMessageComponent', () => {
  let component: NoRecordsFoundMessageComponent;
  let fixture: ComponentFixture<NoRecordsFoundMessageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NoRecordsFoundMessageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NoRecordsFoundMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
