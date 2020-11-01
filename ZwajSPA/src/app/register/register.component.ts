import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { forEach } from '@angular/router/src/utils/collection';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter() ; 
    model:any={} ; 

  constructor(private authService : AuthService) { 

  }

  ngOnInit() {
  }

  register() 
  {
    this.authService.register(this.model).subscribe(
      ()=>{console.log("تم الاشتراك بنجاح");} ,
      error=>{
        
        console.log(error)
        console.log('خطا')
      ;}
    )
  }
  cancel() {
    
    this.cancelRegister.emit(false);
    console.log('ليس الأن') ;
  }

}
