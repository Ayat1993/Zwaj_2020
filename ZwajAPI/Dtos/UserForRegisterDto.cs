using System;
using System.ComponentModel.DataAnnotations;

namespace ZwajAPI.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string UserName { get; set; }

        
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="يجب ان تكون كلمة المرور لا تزيد عن 8 حروف ولا تقل عن اربعة")]
        public string Password { get; set; }
        [Required]
         public string Gender { get; set; }
         [Required]
         public DateTime DateOfBirth  { get; set; }
         [Required]
         public string KnownAs { get; set; }

          public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public UserForRegisterDto()
        {
            Created = DateTime.Now ; 
            LastActive=DateTime.Now ; 
        }

    }
}