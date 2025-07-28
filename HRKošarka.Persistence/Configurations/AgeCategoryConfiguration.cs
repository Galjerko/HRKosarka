using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class AgeCategoryConfiguration : IEntityTypeConfiguration<AgeCategory>
    {
        public void Configure(EntityTypeBuilder<AgeCategory> builder)
        {
            // Configure unique constraints
            builder.HasIndex(e => e.Code).IsUnique();
            builder.HasIndex(e => e.Name).IsUnique();

            // Seed data with static DateTime values
            builder.HasData(
                new AgeCategory
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "U9",
                    Code = "U9",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "U11",
                    Code = "U11",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "U13",
                    Code = "U13",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "U15",
                    Code = "U15",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "U17",
                    Code = "U17",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Name = "Juniori",
                    Code = "JUNIORI",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Name = "Seniori",
                    Code = "SENIORI",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Name = "Juniorke",
                    Code = "JUNIORKE",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                },
                new AgeCategory
                {
                    Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    Name = "Seniorke",
                    Code = "SENIORKE",
                    DateCreated = new DateTime(2025, 7, 25, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
