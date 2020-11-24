using System;
using System.Collections.Generic;

namespace ZwajAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName  { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
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


        








    }
}