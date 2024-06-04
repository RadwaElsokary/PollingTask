using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PollingTask.Domain.Model;
using PollingTask.Repository.Seeding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollingTask.Repository
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.SeedsAdmin();
            //modelBuilder.SeedRole();
        }

        public virtual DbSet<Poll> Polls { set; get; }
        public virtual DbSet<Question> Questions { set; get; }
        public virtual DbSet<Answer> Answers { set; get; }
        public virtual DbSet<UserAnswer> UserAnswers { set; get; }



    }

}
