import { TestBed } from '@angular/core/testing';

import { AppShareService } from './app-share.service';

describe('AppShareService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AppShareService = TestBed.get(AppShareService);
    expect(service).toBeTruthy();
  });
});
