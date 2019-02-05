import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InviteRequestComponent } from './invite-request.component';

describe('InviteRequestComponent', () => {
  let component: InviteRequestComponent;
  let fixture: ComponentFixture<InviteRequestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InviteRequestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InviteRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
