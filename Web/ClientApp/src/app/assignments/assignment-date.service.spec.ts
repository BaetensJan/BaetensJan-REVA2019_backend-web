import {TestBed} from "@angular/core/testing";

import {AssignmentDataService} from "./assignment-data.service";

describe('AnswerDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AssignmentDataService = TestBed.get(AssignmentDataService);
    expect(service).toBeTruthy();
  });
});
