using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ZwajAPI.Models
{
    public class User : IdentityUser<int>
    {
        public string Gender { get; set; }
         public DateTime DateOfBirth { get; set; }
         public string KnownAs { get; set; }
          public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
         public ICollection<Photo>  Photos { get; set; }

         public ICollection<Like> Likers { get ; set ; }
         //قائمة المعجبين 
         //مثلا احمد وعمر وياسر معجبين بمنى  هذه قائمة منفصلة 

         public ICollection<Like> Likees { get ; set  ;}
         //قائمة من المعجب بهم 
         // مثلا منى تعجب بعدة مشتركين مثل رامي وسامي وسيف هذه قائمة مختلفة 

          public ICollection<Message> MessagesSent { get ; set  ;}
          // قائمة الرسائل المرسلة مجموعة
          public ICollection<Message> MessagesReceived { get ; set  ;}

          // قائمة بالرسائل المستلمة
          public ICollection<UserRole> UserRoles { get ; set  ;}

          



        








    }
}