import {TestBed} from "@angular/core/testing";

import {CategoryShareService} from "./category-share.service";

describe('CategoryShareService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CategoryShareService = TestBed.get(CategoryShareService);
    expect(service).toBeTruthy();
  });
});
