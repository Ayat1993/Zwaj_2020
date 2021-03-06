import { THIS_EXPR } from '@angular/compiler/src/output/output_ast';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Photo } from 'src/app/_model/photo';
import { User } from 'src/app/_model/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  created:string ;
   age:string ; 
   option={weekday:'long',year:'numeric',month :'long',day :'numeric'}

  @ViewChild('editForm') editForm : NgForm 
  @HostListener('window:beforeunload',['$event'])
  unLoadNotification($event:any) {
    if(this.editForm.dirty){
      $event.returnValue=true ; 
    }

  }

  constructor(private route : ActivatedRoute ,private alertify : AlertifyService , private userService:UserService , private authService :AuthService) { }
  user:User;
  photoUrl:string ; 

  ngOnInit(){

    this.route.data.subscribe(data=>
      {this.user=data['user']}

    );
    this.authService.currentPhotoUrl.subscribe(photo=>
      this.photoUrl=photo
   );
  this.created = new Date(this.user.created).toLocaleString('ar-EG',this.option).replace('،','') ; 
  this.age=this.user.age.toLocaleString('ar-EG');

  }
  updateUser(){
    
    this.userService.updateUser(this.authService.decodedToken.nameid,this.user).subscribe(
      () => {
      this.alertify.success("تم تعديل الملف الشخصي بنجاح");

      this.editForm.reset(this.user) ;} ,
      error=> {this.alertify.error(error);}
    ) ;

  

  }
  updateMainPhoto(photUrl:string)
  {
    this.user.photoURL=photUrl ; 

  
  }

}
