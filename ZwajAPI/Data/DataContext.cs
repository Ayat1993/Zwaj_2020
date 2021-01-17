using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZwajAPI.Models;

namespace ZwajAPI.Data
{
    public class DataContext : IdentityDbContext<User,Role,int,IdentityUserClaim<int>,
    UserRole,IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        public DbSet<Value> Values { get ; set ; }
      //e  public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get ; set; }
        public DbSet<Like> Likes { get ; set; }
         public DbSet<Message> Messages { get ; set; }

         public DbSet<Payment> Payments { get ; set ;}



        //EF - Core Fluent API ( )
        // علاقة many to Many __ مهم جدا 
        //تعديل في اي كلاس
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder) ; 
            builder.Entity<UserRole>(
                userRole =>
                { 
                  userRole.HasKey(ur =>  new {ur.UserId , ur.RoleId})  ;
                  userRole.HasOne(ur=>ur.Role)
                  .WithMany(r=>r.UserRoles)
                  .HasForeignKey(ur=>ur.RoleId)
                  .IsRequired();
                   

                  userRole.HasOne(ur=>ur.User)
                  .WithMany(r=>r.UserRoles)
                  .HasForeignKey(ur=>ur.UserId)
                  .IsRequired() ;

                }
            );
            // builder.Entity<Role>().HasData(new Role{Id = 1,Name = "Admin",NormalizedName = "ADMIN"},
            //                            new Role{Id = 2,Name = "Moderator",NormalizedName = "MODERATOR"},
            //                            new Role{Id = 3,Name = "Member",NormalizedName = "MEMBER"},
            //                            new Role{Id = 4,Name = "VIP",NormalizedName = "VIP"}
            //                           );
            // var hasher = new PasswordHasher<User>();
            //  builder.Entity<User>().HasData(new User
            // {
            //     Id = 1,
            //     UserName = "admin",
            //     NormalizedUserName = "ADMIN",
            //     Email = "admin@zwaj.com",
            //     NormalizedEmail = "admin@zwaj.com",
            //     EmailConfirmed = true,
            //     PasswordHash = hasher.HashPassword(null, "password"),
            //     Created = DateTime.Now,
            //     LastActive = DateTime.Now,
            //     DateOfBirth = new DateTime(1981, 1, 25),
            //     Gender = "رجل"
            // });
            //  builder.Entity<UserRole>().HasData(new UserRole
            // {
            //     RoleId = 1,
            //     UserId = 1
            // });



            builder.Entity<Like>().
            HasKey(k=>new {k.LikerId , k.LikeeId}) ;
            builder.Entity<Like>().
            HasOne(l=>l.Likee)
            .WithMany(u=>u.Likers)
            .HasForeignKey(l=>l.LikeeId)
            .OnDelete(DeleteBehavior.Restrict) ; 

            builder.Entity<Like>().
            HasKey(k=>new {k.LikerId , k.LikeeId}) ;
            builder.Entity<Like>().
            HasOne(l=>l.Liker).WithMany(u=>u.Likees).HasForeignKey(l=>l.LikerId)
            .OnDelete(DeleteBehavior.Restrict) ; 



           ////////////////////////Messages /////////////////////////////


           builder.Entity<Message>().
            HasOne(m=>m.Sender)
            .WithMany(u=>u.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict) ; 

            builder.Entity<Message>().
            HasOne(m=>m.Recipient).
            WithMany(u=>u.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict) ;

             builder.Entity<Photo>()
             .HasQueryFilter(p=>p.IsApproved) ;



        }


        
        
    }
}