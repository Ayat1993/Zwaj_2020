using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZwajAPI.Data;
using ZwajAPI.Dtos;
using ZwajAPI.Models;

namespace ZwajAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;

        }


        [HttpPost("register")]
         public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
         {

             var aoooo="gggg";
             userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower() ;
             if(await _repo.UserExists(userForRegisterDto.UserName) ) 
             return BadRequest(" هذا المستخدم مستخدم من قبل ");
             var userToCreate = new User { UserName = userForRegisterDto.UserName  } ; 
             var UserCreate =  await _repo.Register(userToCreate , userForRegisterDto.Password) ; 
             return StatusCode(201) ; 
             
         }


    }
}