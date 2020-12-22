import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap';
import { User } from 'src/app/_model/user';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users : User[] ; 
  bsModalRef: BsModalRef;

  constructor(private adminServiec : AdminService , private alertfiy:AlertifyService ,private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles() ;
  }
  getUsersWithRoles()
  {
    this.adminServiec.getUsersWithRoles().subscribe((users:User[])=>
    {
      this.users = users ;


    },
   error=>
   {
     this.alertfiy.error('حدثت مشكلة في اضافة المستخدمين');

   }
    
    );

  }

  editRolesModal(user :User)
  {
    const initialState = 
    {
      user ,
      roles: this.getRolesArray(user) 

  
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    this.bsModalRef.content.updateSelectedRoles
      .subscribe((values) =>
       {
         const roleToUpdate = 
         {
           roleNames:[...values.filter(el => el.checked ===true).map(el=>el.value)]

         }
         if(roleToUpdate)
         {
           this.adminServiec.updateUserRoles(user,roleToUpdate).subscribe(()=>
           {
             user.roles=[...roleToUpdate.roleNames];
           },
           error=>
           {
             this.alertfiy.error(error);

           }
           
           );

         }

         console.log(roleToUpdate) ;

      });
    

  }
  private getRolesArray(user) {
    const roles = [];
    const userRoles = user.roles as any[];
    const availableRoles: any[] = [
      {name: 'مدير النظام', value: 'Admin'},
      {name: 'مشرف', value: 'Moderator'},
      {name: 'عضو', value: 'Member'},
      {name: 'مشترك', value: 'VIP'},
    ];

    availableRoles.forEach(aRole=>{
      let isMatch =false;
      userRoles.forEach(uRole=>{
        if(aRole.value===uRole){
          isMatch=true;
          aRole.checked = true;
          roles.push(aRole);
          return;
         }
      })
      if(!isMatch){
        aRole.checked=false;
        roles.push(aRole);
      }
    })
    return roles;
  }

}
