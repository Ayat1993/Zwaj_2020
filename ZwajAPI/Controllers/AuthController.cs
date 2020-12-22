using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ZwajAPI.Data;
using ZwajAPI.Dtos;
using ZwajAPI.Models;

namespace ZwajAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       // private readonly IAuthRepository _repo;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController( IConfiguration configuration, IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager)
        {
             _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
           /*  userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest(" هذا المستخدم مستخدم من قبل "); */
            var userToCreate = Mapper.Map<User>(userForRegisterDto);
            var resulte = await _userManager.CreateAsync(userToCreate,userForRegisterDto.Password) ;
            var userToReturn = _mapper.Map<UserForDetailsDto>(userToCreate);

            //var UserCreate = await _repo.Register(userToCreate, userForRegisterDto.Password);
            //var userToReturn = _mapper.Map<UserForDetailsDto>(UserCreate);
            //  return Ok(userToReturn) ;
            if(resulte.Succeeded)
            {
             return CreatedAtRoute("GetUser", new { controller = "Users", id = userToCreate.Id }, userToReturn);

            }

            var errors = resulte.Errors  ;
           
            string Description = "" ; 
            if(errors!=null)
            {
            foreach( var error in errors)
            {
               if(error.Code.ToString()==nameof(IdentityErrorDescriber.DuplicateUserName))
                 Description= "موجود سابقا '"+userToCreate+"' اسم المستخدم " ;
               if(error.Code.ToString()==nameof(IdentityErrorDescriber.PasswordRequiresLower))
                Description= "يجب ان تحتوي كلمة المرور على الاقل حرف صغير واحد"; 

            }

            }


            return BadRequest(Description) ; 

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            //throw new Exception("Api Sys nooo") ;
            /* var userFromRep = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);
            if (userFromRep == null) return Unauthorized(); */
            /*  var claims = new[] {
                   new Claim(ClaimTypes.NameIdentifier , userFromRep.Id.ToString()) ,
                   new Claim(ClaimTypes.Name , userFromRep.UserName)

               };
             var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

             var cards = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
             var tokenDescripror = new SecurityTokenDescriptor
             {
                 Subject = new ClaimsIdentity(claims),
                 Expires = DateTime.Now.AddDays(1),
                 SigningCredentials = cards

             };

             var tokenHandler = new JwtSecurityTokenHandler();
             var token = tokenHandler.CreateToken(tokenDescripror); */
             var user = await _userManager.FindByNameAsync(userForLoginDto.username) ; 
             var result = await _signInManager.CheckPasswordSignInAsync(user,userForLoginDto.password,false) ; 
             if(result.Succeeded)
             {
                 var appUser = await _userManager.Users.Include(p=>p.Photos).FirstOrDefaultAsync(
                     u=> u.NormalizedUserName == userForLoginDto.username.ToUpper() 
                 ) ;
                 var userToReturn = _mapper.Map<UserForListDto>(appUser);

                 return Ok(new { token = GenerateJwtToken(appUser).Result, user=userToReturn });



             }
             return Unauthorized() ;

        }
        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim> {
                  new Claim(ClaimTypes.NameIdentifier , user.Id.ToString()) ,
                  new Claim(ClaimTypes.Name , user.UserName)

              };

            var roles =  await _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                claims.Add(new Claim (ClaimTypes.Role,role));

            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var cards = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescripror = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cards

            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescripror);
            return tokenHandler.WriteToken(token);


        }




    }
}