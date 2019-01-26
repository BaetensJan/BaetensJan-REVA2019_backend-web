import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExhibitorComponent } from './exhibitor.component';

describe('ExhibitorComponent', () => {
  let component: ExhibitorComponent;
  let fixture: ComponentFixture<ExhibitorComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExhibitorComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExhibitorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
