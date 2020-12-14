using Microsoft.EntityFrameworkCore;
using ZwajAPI.Models;

namespace ZwajAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
        public DbSet<Value> Values { get ; set ; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get ; set; }
        public DbSet<Like> Likes { get ; set; }
         public DbSet<Message> Messages { get ; set; }

         public DbSet<Payment> Payments { get ; set ;}



        //EF - Core Fluent API ( )
        // علاقة many to Many __ مهم جدا 
        //تعديل في اي كلاس
        protected override void OnModelCreating(ModelBuilder builder)
        {
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



        }


        
        
    }
}