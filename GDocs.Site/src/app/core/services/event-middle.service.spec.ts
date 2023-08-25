import { TestBed } from '@angular/core/testing';

import {  EventMiddleService } from './event-middle.service';

describe('EventServiceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: EventMiddleService = TestBed.get(EventMiddleService);
    expect(service).toBeTruthy();
  });
});
