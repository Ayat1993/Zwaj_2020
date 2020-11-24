using System.Collections.Generic;
using System.Threading.Tasks;
using ZwajAPI.Helpers;
using ZwajAPI.Models;

namespace ZwajAPI.Data
{
    public interface IZwajRepository
    {
        void Add <T>(T entity) where T:class ; 
        void Delete <T>(T entity) where T:class ; 
        Task<bool> SaveAll() ; 
        Task<PagedList<User>> GetUsers(UserParams userParams) ; 
        Task<User> GetUser(int id) ;  
        Task<Photo> GetPhoto(int id) ; 
        Task<Photo> GetMainPhotoForUser(int userId) ;

        Task<Like> GetLike(int userId , int recipientId);
        


         
    }
}