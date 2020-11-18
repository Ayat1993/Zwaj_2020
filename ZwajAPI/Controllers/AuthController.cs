using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {

            userForRegisterDto.UserName = userForRegisterDto.UserName.ToLower();
            if (await _repo.UserExists(userForRegisterDto.UserName))
                return BadRequest(" هذا المستخدم مستخدم من قبل ");
            var userToCreate =  Mapper.Map<User>(userForRegisterDto)  ;
            
            var UserCreate = await _repo.Register(userToCreate, userForRegisterDto.Password);
            var userToReturn = _mapper.Map<UserForDetailsDto>(UserCreate) ;
           //  return Ok(userToReturn) ;
            return CreatedAtRoute("GetUser", new {controller="Users",id=UserCreate.Id},userToReturn);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {

            //throw new Exception("Api Sys nooo") ;
            var userFromRep = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);
            if (userFromRep == null) return Unauthorized();
            var claims = new[] {
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
            var token = tokenHandler.CreateToken(tokenDescripror);
            var user= _mapper.Map<UserForListDto>(userFromRep);
            return Ok(new{ token = tokenHandler.WriteToken(token),user} );
             
               

            



            //return StatusCode(500,"API Is very tired") ; 











        }



    }
}