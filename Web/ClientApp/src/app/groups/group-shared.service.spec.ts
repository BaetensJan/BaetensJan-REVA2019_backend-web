import { TestBed } from '@angular/core/testing';

import { GroupSharedService } from './group-shared.service';

describe('GroupSharedService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: GroupSharedService = TestBed.get(GroupSharedService);
    expect(service).toBeTruthy();
  });
});
