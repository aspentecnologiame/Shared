import { TestBed } from '@angular/core/testing';

import { ExibicaoDeAlertaService } from './exibicao-de-alerta.service';

describe('ExibicaoDeAlertaService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ExibicaoDeAlertaService = TestBed.get(ExibicaoDeAlertaService);
    expect(service).toBeTruthy();
  });
});
