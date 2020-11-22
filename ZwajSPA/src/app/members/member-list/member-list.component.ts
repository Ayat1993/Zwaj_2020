import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginationResult } from 'src/app/_model/Pagination';
import { User } from '../../_model/user';
import { AlertifyService } from '../../_services/alertify.service';
import { UserService } from '../../_services/user.service';



@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users:User[] ;
  user = JSON.parse(localStorage.getItem('user')) ;
  genderList=[{value:'1',display:'رجال'},{value:'2',display:'نساء'}];
  userParams:any ={} ;

  pagination :Pagination;

  constructor(private userService :UserService , private alertify : AlertifyService, private route :ActivatedRoute) { }

  ngOnInit() 
  {
    this.route.data.subscribe(data=>
      {
        this.users=data['users'].result;
        this.pagination = data['users'].pagination 
      }
    ) ;
    this.userParams.gender = this.user.gender==='1'?'2':'1'
    this.userParams.maxAge = 99 ; 
    this.userParams.minAge = 21 ;
    this.userParams.pageSize=10 ;
    this.userParams.orderBy ='lastActive' ;
    


   

  }
  resetFilter(){
    this.userParams.gender = this.user.gender==='1'?'2':'1';
    this.userParams.maxAge = 99 ; 
    this.userParams.minAge = 21 ;
    this.userParams.pageSize=10 ; 
    this.userParams.orderBy ='lastActive' ; 
    this.loadUsers();

  }


  loadUsers(){

    this.pagination.itemsPerPage=this.userParams.pageSize ;

    this.userService.getUsers(this.pagination.currentPage,this.pagination.itemsPerPage,this.userParams).subscribe((res:PaginationResult<User[]>)=>
    {
      this.users = res.result ;
      this.pagination= res.pagination ;
      
    },
    error =>{
      this.alertify.error(error) ;

    }
    )

   

  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    console.log(this.pagination.currentPage) ;
    this.loadUsers();
  }

}
