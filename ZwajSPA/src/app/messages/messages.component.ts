import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from '../_model/Message';
import { Pagination, PaginationResult } from '../_model/Pagination';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages:Message[] ; 
  pagination :Pagination ;
  messageType = 'Unread' ; 

  constructor(private userService :UserService , private authService :AuthService,private route :ActivatedRoute , private alertify :AlertifyService) { }

  ngOnInit() 
  {
    this.route.data.subscribe(
      data=> 
      {
        this.messages = data['messages'].result ; 
        this.pagination = data['messages'].pagination ; 

      },
      error=>{}
      
    )
  }
  loadMessages()
  {
    this.userService.getMessages(this.authService.decodedToken.nameid,this.pagination.currentPage,this.pagination.itemsPerPage,this.messageType).subscribe(
      (res:PaginationResult<Message[]>)=>
      {
        this.messages =res.result ; 
        this.pagination = res.pagination ; 

      },
      error=>
      {
        this.alertify.error(error);
        

      }
      
    );

  }
  pageChanged(event:any)
  {
    this.pagination.currentPage = event.page ; 
    this.loadMessages()


  }
  deleteMessage(id:number)
  {
    this.alertify.confirm('هل انت متأكد من حذف الرسالة',()=>
    {
      this.userService.deleteMessage(id, this.authService.decodedToken.nameid).subscribe(
        ()=> 
        { 
          this.messages.splice(this.messages.findIndex(m=>m.id == id ),1) ;
          this.alertify.success('تم حذف الرسالة بنجاح');
        },

        error=>
        {
          this.alertify.error(error);
          
        }

      );

    })

  }

}
