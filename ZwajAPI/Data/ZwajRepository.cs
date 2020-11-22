using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZwajAPI.Helpers;
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

        async Task<PagedList<User>> IZwajRepository.GetUsers(UserParams userParams)
        {
            var users =   _context.Users.Include(u=>u.Photos).OrderByDescending(u=>u.LastActive).AsQueryable() ; 
            users=users.Where(u=>u.Id !=userParams.UserId );
            users=users.Where(u=>u.Gender ==userParams.Gender );
            if(userParams.MinAge >=21 || userParams.MaxAge <=99){
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge-1) ; 
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge) ;
                users=users.Where(u=>u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob  );

        
            }
            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created" :
                    users=users.OrderByDescending(u=>u.Created) ; 
                    break ; 
                    default :
                    users=users.OrderByDescending(u=>u.LastActive) ; 
                    break ;

                }


            }
            return await PagedList<User>.CreateAsync(users ,userParams.PageNumber,userParams.PageSize) ; 

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