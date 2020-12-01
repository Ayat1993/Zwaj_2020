import { Message } from "@angular/compiler/src/i18n/i18n_ast";
import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError } from "rxjs/operators";
import { AlertifyService } from "../_services/alertify.service";
import { AuthService } from "../_services/auth.service";
import { UserService } from "../_services/user.service";

@Injectable()
export class MessageResolver implements Resolve<Message[]>{
    pageNumber = 1  ; 
    pageSize = 10 ;
    messageType ='Unread' ; 

    constructor(private authServiec : AuthService ,  private userServiec :UserService , private router:Router , private alertify : AlertifyService){    
    }
    resolve(route:ActivatedRouteSnapshot):Observable<Message[]>
    {
        return this.userServiec.getMessages(this.authServiec.decodedToken.nameid,this.pageNumber,this.pageSize,this.messageType).pipe(
            catchError(error =>
                {
                    this.alertify.error('يوجد مشكلة في عرض الرسائل ');
                    this.router.navigate(['']) ;
                    return of(null) ; 

                })
        )

    }

}