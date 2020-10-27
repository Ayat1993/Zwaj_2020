using System.Threading.Tasks;
using ZwajAPI.Models;

namespace ZwajAPI.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user , string Password) ;
         Task<User> Login(string username , string password) ;  
         Task<bool> UserExists(string username) ; 
    }
}