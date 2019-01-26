import { TestBed } from '@angular/core/testing';

import { QuestionSharedService } from './question-shared.service';

describe('QuestionSharedService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: QuestionSharedService = TestBed.get(QuestionSharedService);
    expect(service).toBeTruthy();
  });
});
