using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZwajAPI.Models;

namespace ZwajAPI.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p=>p.Photos).FirstOrDefaultAsync(x=>x.UserName ==username) ;
            if(user==null) return null ; 
           /*  if(!VerifyPasswordHash(password ,user.PasswordSalt ,user.PasswordHash))
            return null ;  */
            return user ; 


        }

        private bool VerifyPasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
              using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)){
                var ComputeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)) ;
                    for( var i =0 ; i < ComputeHash.Length ;  i++)
                    {
                        if(ComputeHash[i] !=passwordHash[i]){
                            return false ; 
                        }

                    } 
                  return true ; 


                 }

        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash , passwordSalt ; 
            CreatePasswordHash(password,out passwordHash , out passwordSalt ) ; 
           /*  user.PasswordSalt =  passwordSalt  ; 
            user.PasswordHash = passwordHash  ;  */
            await _context.Users.AddAsync(user)  ; 
            await _context.SaveChangesAsync() ; 
            return user ; 


        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out  byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512()){
               passwordSalt= hmac.Key ; 
               passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)) ; 


                 }
        }

        public async Task<bool> UserExists(string username)
        {
             if (await _context.Users.AnyAsync( X=> X.UserName==username)) 
            {
               return true ; 
            }
            return false ; 
        }
    }
}