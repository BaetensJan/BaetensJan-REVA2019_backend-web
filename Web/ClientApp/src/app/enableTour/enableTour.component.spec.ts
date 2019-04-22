import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { EnableTourComponent } from './enableTour.component';

describe('EnableTourComponent', () => {
  let component: EnableTourComponent;
  let fixture: ComponentFixture<EnableTourComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EnableTourComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EnableTourComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
