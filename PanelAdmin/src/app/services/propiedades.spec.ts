import { TestBed } from '@angular/core/testing';

import { Propiedades } from './propiedades';

describe('Propiedades', () => {
  let service: Propiedades;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Propiedades);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
