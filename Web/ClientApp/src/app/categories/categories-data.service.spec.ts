import {TestBed} from '@angular/core/testing';

import {CategoriesDataService} from './categories-data.service';

describe('CategoriesDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CategoriesDataService = TestBed.get(CategoriesDataService);
    expect(service).toBeTruthy();
  });
});
