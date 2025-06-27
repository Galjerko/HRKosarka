using HRKošarka.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Identity.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder.HasData(
                        new ApplicationUser
                        {
                            Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                            Email = "admin@gmail.com",
                            NormalizedEmail = "ADMIN@GMAIL.COM",
                            FirstName = "System",
                            LastName = "Admin",
                            UserName = "admin",
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAEAACcQAAAAEHxImQYWZaZA56CDlacBJI0QzvvcghOKjGC2LMVMMUvxIExImNLimBfWNOmZXxglHg==",
                            EmailConfirmed = true,
                            ConcurrencyStamp = "admin-concurrency-stamp", 
                            SecurityStamp = "admin-security-stamp"        
                        }
                    );
        }
    }
}
