import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Directive({
  selector: '[hasRole]'
})
export class HasRoleDirective implements OnInit {
@Input() hasRole : string[] ;
isVisible = false ; 
  constructor(private viewContainerRef:ViewContainerRef, private templateRef:TemplateRef<any>, private authSerives:AuthService) 
  { 

  }
  ngOnInit()
  {
    const userRoles = this.authSerives.decodedToken.role as Array<string>;
    if(!userRoles)
    {
      this.viewContainerRef.clear() ;

    }
    if(this.authSerives.roleMatch(this.hasRole))
    {
      if(!this.isVisible){
        this.isVisible = true ;
        this.viewContainerRef.createEmbeddedView(this.templateRef);
      }else 
      {
        this.isVisible = false ; 
        this.viewContainerRef.clear() ; 

      }

    }

  }

}
