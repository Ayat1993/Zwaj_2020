import { AfterViewChecked, Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_model/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { NgxGalleryOptions, NgxGalleryImage, NgxGalleryAnimation } from 'ngx-gallery';
import { TabsetComponent } from 'ngx-bootstrap';
import { AuthService } from 'src/app/_services/auth.service';


@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit , AfterViewChecked{
  @ViewChild('memberTabs') memberTabs: TabsetComponent;

user :User ; 
paid : boolean = false ; 
created:string ;
age:string ; 
showIntro:boolean=true ;
showLook: boolean=true ; 
showInterests:boolean=true  ; 

option={weekday:'long',year:'numeric',month :'long',day :'numeric'}
galleryOptions: NgxGalleryOptions[];
galleryImages: NgxGalleryImage[];
  constructor( private authService:AuthService, private userService :UserService, private alertify : AlertifyService,private route :ActivatedRoute) { }
  ngAfterViewChecked(): void
   {
     setTimeout(() =>
      {
        this.paid = this.authService.paid ; 
       
     }, 0);

  }

  ngOnInit(){
    this.paid = this.authService.paid ; 

    //this.loadUser();
    this.route.data.subscribe(data=>{
      this.user=data['user'] ; 
    });
    this.route.queryParams.subscribe(
      params=>
      {
        const selectTab = params['tab'] ; 
        this.memberTabs.tabs[selectTab>0?selectTab:0].active=true ;
      }
    );
    this.galleryOptions = [
      {
          width: '500px',
          height: '500px',
          imagePercent:100 ,
          thumbnailsColumns: 4,
          imageAnimation: NgxGalleryAnimation.Slide,
          preview:false 

      } 

  ];

  this.galleryImages = this.getImages();
  this.created = new Date(this.user.created).toLocaleString('ar-EG',this.option).replace('،','') ; 
  this.age=this.user.age.toLocaleString('ar-EG');
  this.showIntro=true ;
  this.showLook = true ;
  this.showInterests =true ; 
    
     
     
  
    
  }
  getImages()
  {
    const imageUrl =[] ;
    for(let i=0 ; i< this.user.photos.length ; i++)
    {
      imageUrl.push({
        small:this.user.photos[i].url,
        medium :this.user.photos[i].url ,
        big:this.user.photos[i].url


      });

    }
    return imageUrl;



  }
  /*
  loadUser(){
      this.userService.getUser(+this.route.snapshot.params['id']).subscribe(
       (user:User)=>{this.user =user;},
       error=>{
         this.alertify.error(error);

       }

     ) ;
  }*/

  selectTab(tabId: number) {
    this.memberTabs.tabs[tabId].active = true;
  }

  sendLike(id:number)
  {
    this.userService.sendLike(this.authService.decodedToken.nameid,id).subscribe(
      ()=>
      {
        this.alertify.success('لقد قمت بالعجاب ب '+this.user.knownAs) ;


      },
     error=>
     {
       this.alertify.error(error);
       

     }
      
      
    )

  }

  deselect()
  {
    this.authService.hubConnection.stop() ;

  }


}
