import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InformatieschermComponent } from './informatiescherm.component';

describe('InformatieschermComponent', () => {
  let component: InformatieschermComponent;
  let fixture: ComponentFixture<InformatieschermComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InformatieschermComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InformatieschermComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
