import {TestBed} from '@angular/core/testing';

import {ExhibitorShareService} from './exhibitor-share.service';

describe('ExhibitorShareService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ExhibitorShareService = TestBed.get(ExhibitorShareService);
    expect(service).toBeTruthy();
  });
});
