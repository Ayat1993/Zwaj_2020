import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { JwtHelperService } from "@auth0/angular-jwt";
import { map } from "rxjs/operators";
import { environment } from "src/environments/environment";
import { User } from "../_model/user";
import { BehaviorSubject } from "rxjs";
import { HubConnection, HubConnectionBuilder } from "@aspnet/signalr";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  jwtHelper = new JwtHelperService();
  baseUrl = environment.apiUrl + "auth/";
  unreadCount = new BehaviorSubject<string>('');
  latestUnreadCount = this.unreadCount.asObservable();
  hubConnection:HubConnection = new HubConnectionBuilder().withUrl("http://localhost:5000/chat").build() ;


  decodedToken: any;
  currentUser: User;
  paid : boolean =false ; 
  
  photoUrl = new BehaviorSubject<string>("../../assets/User.png");
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}


  changeMemberPhoto(newPhotoUrl: string) 
  {
    this.photoUrl.next(newPhotoUrl);
  }


  login(model: any) {
    return this.http.post(this.baseUrl + "login", model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem("token", user.token);
          localStorage.setItem("user", JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          console.log(this.decodedToken);
          this.changeMemberPhoto(this.currentUser.photoURL);
        }
      })
    );
  }



  register(user: User)
  {
    return this.http.post(this.baseUrl + "register", user);
  }


  loggedIn(): boolean
   {
    try {
      const token = localStorage.getItem("token");
      return !this.jwtHelper.isTokenExpired(token);
    } catch {
      return false;
    }
  }
}
