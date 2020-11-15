import { Component ,OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_model/user';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  
  jwtHelpers = new JwtHelperService() ; 

  constructor(public authService : AuthService) { 

  }

  ngOnInit() {
    const token = localStorage.getItem('token') ;
    const user :User = JSON.parse(localStorage.getItem('user')) ; 
    if(token){
      this.authService.decodedToken = this.jwtHelpers.decodeToken(token) ;


    }
    if(user){
          this.authService.currentUser = user ; 
          this.authService.changeMemberPhoto(this.authService.currentUser.photoURL) ;

    }

    

  }

}
