import { TestBed } from '@angular/core/testing';

import { InvoiceApi } from './invoice-api';

describe('InvoiceApi', () => {
  let service: InvoiceApi;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(InvoiceApi);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
