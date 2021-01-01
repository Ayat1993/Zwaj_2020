import { Component, OnInit, ViewChild } from '@angular/core';
import { AllMemberReportComponent } from 'src/app/_reports/all-member-report/all-member-report.component';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {
 @ViewChild('report') report :AllMemberReportComponent ; 
  constructor(public authService:AuthService ) { }

  ngOnInit() {
  }
  printAll()
  {
    this.report.printAll();

  }

}
