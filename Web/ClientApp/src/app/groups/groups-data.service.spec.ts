import {TestBed} from '@angular/core/testing';

import {GroupsDataService} from './groups-data.service';

describe('GroupsDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GroupsDataService = TestBed.get(GroupsDataService);
    expect(service).toBeTruthy();
  });
});
