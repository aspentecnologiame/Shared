import { TestBed } from '@angular/core/testing';

import { GDocsApiEndpointService } from './gdocs-api-endpoint.service';

describe('GDocsApiEndpointService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GDocsApiEndpointService = TestBed.get(GDocsApiEndpointService);
    expect(service).toBeTruthy();
  });
});
