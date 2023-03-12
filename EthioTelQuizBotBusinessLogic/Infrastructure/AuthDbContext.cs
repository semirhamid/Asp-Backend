using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EthioTelQuizBotBusinessLogic
{
    public class AuthDbContext : IdentityDbContext<AppUser>
    {
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Problem> Problems { get; set; }
        public virtual DbSet<Subscriber> Subscribers { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.Entity<AppUser>(b =>
            //    {
            //        b.Property(u => u.Phone).IsRequired(false);
            //        b.Property(u => u.UserName).IsRequired(true);
            //        b.Property(u => u.Email).IsRequired(true);
            //        b.ToTable("AspNetUsers");
            //    }
            //);

   

            base.OnModelCreating(builder);

        }
    }
}
