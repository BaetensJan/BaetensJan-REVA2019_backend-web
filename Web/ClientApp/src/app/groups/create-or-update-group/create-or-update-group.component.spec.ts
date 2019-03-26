import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import {CreateOrUpdateGroupComponent} from "./create-or-update-group.component";

describe('UpdateGroupComponent', () => {
  let component: CreateOrUpdateGroupComponent;
  let fixture: ComponentFixture<CreateOrUpdateGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateOrUpdateGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateOrUpdateGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
