using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZwajAPI.Models;

namespace ZwajAPI.Data
{
    public class ZwajRepository : IZwajRepository
    {
        private readonly DataContext _context;
        public ZwajRepository(DataContext context)
        {
            _context = context;

        }
        void IZwajRepository.Add<T>(T entity)
        {
            _context.Add(entity) ; 
           
        }

        void IZwajRepository.Delete<T>(T entity)
        {
            _context.Remove(entity) ; 


        }

       async Task<User> IZwajRepository.GetUser(int id)
        {
            var user = await _context.Users.Include(u=>u.Photos).FirstOrDefaultAsync(x=>x.Id ==id) ; 
            return user ; 
        }

        async Task<IEnumerable<User>> IZwajRepository.GetUsers()
        {
            var users = await  _context.Users.Include(u=>u.Photos).ToListAsync() ; 
            return users ; 

        }

        public async Task<bool> SaveAll()
        {
           return await _context.SaveChangesAsync()>0 ; 
        }

        public async Task<Photo> GetPhoto(int id)
        {
           var photo =  await _context.Photos.FirstOrDefaultAsync(p=> p.Id == id)  ; 
           return photo ; 
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(p=>p.UserId==userId).FirstOrDefaultAsync(p=>p.IsMain) ;

            
        }
    }
}