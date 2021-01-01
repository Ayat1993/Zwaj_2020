import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AllMemberReportComponent } from './all-member-report.component';

describe('AllMemberReportComponent', () => {
  let component: AllMemberReportComponent;
  let fixture: ComponentFixture<AllMemberReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AllMemberReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AllMemberReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
