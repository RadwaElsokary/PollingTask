using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PollingTask.Repository.Seeding
{
    public static class SeedRoles
    {
            private static List<IdentityRole> roles;

            public static void SeedRole(this ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<IdentityRole>().HasData(
                    roles = new List<IdentityRole>()
                    {
                    new IdentityRole(){Id="1" ,Name="Admin",NormalizedName="ADMIN"},
                    new IdentityRole(){Id="2" ,Name="User",NormalizedName="USER"},

                    });
            }
        }
    }
