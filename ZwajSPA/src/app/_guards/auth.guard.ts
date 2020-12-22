import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService : AuthService, private router :Router , private alertify : AlertifyService )
  {
    

  }
  canActivate(next:ActivatedRouteSnapshot): boolean {
    // فتح المسار او لا 
    const roles = next.firstChild.data['roles']  as Array<string>
    if(roles)
    {
      const match = this.authService.roleMatch(roles) ;
      if(match)
      {
        return true ; 

      }
      else
      {
        this.router.navigate(['/members']);
        this.alertify.error('غير مسموح لك بالدخول') ;

      }
    }
    if (this.authService.loggedIn()) {
      this.authService.hubConnection.stop();
      return true ;  
    }
     this.alertify.error('يجب تسجيل الدخول اولا'); 
     this.router.navigate(['']) ;
     return false ; 


    

    
   
  }
}
