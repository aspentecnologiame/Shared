import { TestBed } from '@angular/core/testing';

import { Fi1548Service } from './FI1548.service';

describe('Fi1548Service', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: Fi1548Service = TestBed.get(Fi1548Service);
    expect(service).toBeTruthy();
  });
});
