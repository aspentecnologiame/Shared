import { TestBed, inject } from '@angular/core/testing';

import { PermitionGuard } from './permition.guard';

describe('PermitionGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PermitionGuard]
    });
  });

  it('should ...', inject([PermitionGuard], (guard: PermitionGuard) => {
    expect(guard).toBeTruthy();
  }));
});
