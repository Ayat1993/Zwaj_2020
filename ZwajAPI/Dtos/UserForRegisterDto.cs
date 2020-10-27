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

    }
}