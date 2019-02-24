import {TestBed} from "@angular/core/testing";

import {RequestsShareService} from "./requests-share.service";

describe('RequestsShareService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RequestsShareService = TestBed.get(RequestsShareService);
    expect(service).toBeTruthy();
  });
});
