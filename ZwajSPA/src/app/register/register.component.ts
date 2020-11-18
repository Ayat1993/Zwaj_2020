import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BsDatepickerConfig, BsLocaleService } from 'ngx-bootstrap';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { defineLocale } from 'ngx-bootstrap/chronos';
import { arLocale } from 'ngx-bootstrap/locale';
import { User } from '../_model/user';
import { Router } from '@angular/router';
defineLocale('ar', arLocale);


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter() ; 
    user :User ;
    registerForm: FormGroup  ;
    bsConfig: Partial<BsDatepickerConfig>;
  locale = 'ar';


  constructor(private authService : AuthService, private router :Router ,private alertify :AlertifyService , private fb:FormBuilder , private localeService: BsLocaleService) { 
    this.localeService.use(this.locale) ; 

  }

  ngOnInit() {
    this.bsConfig ={
      containerClass :'theme-red',
      showWeekNumbers:false 
    }
 
    this.createRegisterForm();

  }
  createRegisterForm(){
    this.registerForm = this.fb.group({
      gender:['رجل'],
      username:['',Validators.required],
      knownAs : ['',Validators.required],
      dateOfBirth :['',Validators.required],
      city :['',Validators.required],
      country:['',Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword :['',Validators.required]
    },{validator:this.passwordMatchValidat})

  }
  passwordMatchValidat(form:FormGroup){
    return form.get('password').value === form.get('confirmPassword').value ? null : {'mismatch':true} ;  

  }

  register() 
  {
    if(this.registerForm.valid)
    {
      this.user = Object.assign({},this.registerForm.value);
      this.authService.register(this.user).subscribe(
        ()=>{
        this.alertify.success('تم الاشتراك بنجاح') ;
      } ,
        error=>{
          
          this.alertify.error(error) ; 
        },
        ()=>{
          this.authService.login(this.user).subscribe(
            ()=>
            {
            this.router.navigate(['/members']) ;
            }

          ) ;

        }
      );


    }
    

    console.log(this.registerForm.value) ;
  }
  cancel() {
    
    this.cancelRegister.emit(false);
    console.log('ليس الأن') ;
  }

}
