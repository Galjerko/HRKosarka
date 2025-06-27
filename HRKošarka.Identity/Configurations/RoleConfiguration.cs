using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Identity.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
               new IdentityRole
               {
                   Id = "cac43a6e-f7bb-4448-baaf-1add431ccbbf",
                   Name = "RegularUser",
                   NormalizedName = "REGULARUSER",
                   ConcurrencyStamp = "regular-user-role-stamp"
               },
               new IdentityRole
               {
                   Id = "c4f21b48-2097-4d98-a590-8b9f937f08e2",
                   Name = "Administrator",
                   NormalizedName = "ADMINISTRATOR",
                   ConcurrencyStamp = "admin-role-stamp"
               },
               new IdentityRole
               {
                   Id = "edf1f7a1-401e-4f20-868e-e3b5c0702630",
                   Name = "ClubManager", 
                   NormalizedName = "CLUBMANAGER", 
                   ConcurrencyStamp = "club-manager-role-stamp"
               }
           );
        }
    }

}
