import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  registerMode :boolean = false ; 
  values : any ={} ; 

  constructor(private http:HttpClient ,private authSerivce :AuthService, private router : Router) { }

  ngOnInit() {
    if(this.authSerivce.loggedIn){
      this.router.navigate(['/members']) ; 

    }
    
  }
  registerToggle(){
    this.registerMode= true  ; 

  }
  
  cancelRegister(mode:boolean){
    console.log(mode) ; 
    this.registerMode=mode ; 

  }


}
