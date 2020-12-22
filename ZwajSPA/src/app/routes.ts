import { Routes } from "@angular/router";
import { AdminPanelComponent } from "./admin/admin-panel/admin-panel.component";
import { HomeComponent } from "./home/home.component";
import { ListsComponent } from "./lists/lists.component";
import { ChangePasswordComponent } from "./members/change-password/change-password.component";
import { MemberDetailComponent } from "./members/member-detail/member-detail.component";
import { MemberEditComponent } from "./members/member-edit/member-edit.component";
import { MemberListComponent } from "./members/member-list/member-list.component";
import { MessagesComponent } from "./messages/messages.component";
import { PaymentComponent } from "./payment/payment.component";
import { AuthGuard } from "./_guards/auth.guard";
import { ChargeGuard } from "./_guards/charge.guard";
import { MessagesGuard } from "./_guards/messages.guard";
import { PreventUnsavedChangesGuard } from "./_guards/prevent-unsaved-changes.guard";
import { ListResolver } from "./_resolvers/list-resolver";
import { MemberDetailResolver } from "./_resolvers/member-detail-resolver";
import { MemberEditResolver } from "./_resolvers/member-edit-resolver";
import { MemberListResolver } from "./_resolvers/member-list-resolver";
import { MessageResolver } from "./_resolvers/Message-Resolver";


export const appRoutes : Routes =[
    {path:'',component:HomeComponent},
    // مسار وهمي 
    //
    
    {path:'' ,runGuardsAndResolvers:'always',canActivate:[AuthGuard],
    children:[
        {path:'members',component:MemberListComponent,resolve:{
            users:MemberListResolver
        }},
        {path:'member/edit',component:MemberEditComponent ,resolve:{
            user:MemberEditResolver
        }, canDeactivate:[PreventUnsavedChangesGuard]},
        
        {path:'memberChange/changePassword',component: ChangePasswordComponent} ,
              
        {path:'members/:id',component:MemberDetailComponent ,resolve:{
            user:MemberDetailResolver
        }},
        {path:'lists',component:ListsComponent,resolve:{users:ListResolver}},
        {path:'messages',component:MessagesComponent,canActivate:[MessagesGuard] ,resolve:{messages:MessageResolver}},
        {path:'charge',component:PaymentComponent,canActivate:[ChargeGuard]},
        {path:'admin',component:AdminPanelComponent,data:{roles:['Admin','Moderator']}}

    ]},
   
    {path:'**',redirectTo:'',pathMatch:'full'}



];
