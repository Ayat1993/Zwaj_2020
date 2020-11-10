/////////////Module**********************Module****************///////////////Module

import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule  } from "@angular/forms";
import { BsDropdownModule, TabsModule } from 'ngx-bootstrap';
import { appRoutes } from './routes';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { HttpClientModule } from "@angular/common/http";


//////////Component**********************Component****************//////////Component

import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { NgxGalleryModule } from 'ngx-gallery';



//////////Services***********************Services****************///////////////Services 

import { AuthService } from './_services/auth.service';
import { ErrorInterceptorProvidor } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { UserService } from './_services/user.service';





//Guard
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';



export function tokenGetter() {
  return localStorage.getItem("token");
}


@NgModule({
  declarations: [						
    AppComponent,
    NavComponent , 
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    ListsComponent,
    MessagesComponent,
    MemberCardComponent,
    MemberDetailComponent
   ],
  imports: [
    BrowserModule ,
    HttpClientModule ,
    FormsModule  ,
    BsDropdownModule.forRoot(),
    RouterModule.forRoot(appRoutes) ,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["localhost:5000"],
        blacklistedRoutes: ["localhost:5000/api/auth"],
      },
    }),
    TabsModule.forRoot() , 
    NgxGalleryModule


  ],
  providers: [
    AuthService ,
    ErrorInterceptorProvidor ,
    AlertifyService ,
    AuthGuard , 
    UserService ,
    MemberDetailResolver,
    MemberListResolver
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
