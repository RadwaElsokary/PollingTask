using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PollingTask.Repository.Seeding
{
    public static class SeedAdmin
    {
        public static void SeedsAdmin(this ModelBuilder modelBuilder)
        {
            {
                var AdminUser = new IdentityUser
                {
                    PhoneNumber = "NULL",
                    Email = "admin@email.com",
                    UserName = "admin@email.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    NormalizedEmail = "ADMIN@EMAIL.COM",
                    NormalizedUserName = "ADMIN@EMAIL.COM",
                };


                var Password = new PasswordHasher<IdentityUser>();
                var hashed = Password.HashPassword(AdminUser, "Admin123###");
                AdminUser.PasswordHash = hashed;

                modelBuilder.Entity<IdentityUser>().HasData(AdminUser);
                modelBuilder.Entity<IdentityUserRole<string>>().HasData(

                        new IdentityUserRole<string>()
                        {
                            RoleId = "1",
                            UserId = AdminUser.Id
                        });
            }
        }
    }
}
    
