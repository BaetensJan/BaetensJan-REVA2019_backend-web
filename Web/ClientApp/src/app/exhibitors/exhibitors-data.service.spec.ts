import {TestBed} from '@angular/core/testing';

import {ExhibitorsDataService} from './exhibitors-data.service';

describe('ExhibitorsDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ExhibitorsDataService = TestBed.get(ExhibitorsDataService);
    expect(service).toBeTruthy();
  });
});
