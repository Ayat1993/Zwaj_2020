import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError } from "rxjs/operators";
import { User } from "../_model/user";
import { AlertifyService } from "../_services/alertify.service";
import { UserService } from "../_services/user.service";

@Injectable()
export class MemberListResolver implements Resolve<User[]>{
    constructor(private userServiec :UserService , private router:Router , private alertify : AlertifyService){    
    }
    resolve(route:ActivatedRouteSnapshot):Observable<User[]>
    {
        return this.userServiec.getUsers().pipe(
            catchError(error =>
                {
                    this.alertify.error('يوجد مشكلة في عرض البيانات ');
                    this.router.navigate(['']) ;
                    return of(null) ; 

                })
        )

    }

}