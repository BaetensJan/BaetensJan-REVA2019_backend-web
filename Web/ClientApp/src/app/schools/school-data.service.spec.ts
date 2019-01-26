import { TestBed } from '@angular/core/testing';

import { SchoolDataService } from './school-data.service';

describe('SchoolDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SchoolDataService = TestBed.get(SchoolDataService);
    expect(service).toBeTruthy();
  });
});
