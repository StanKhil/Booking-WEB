using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Booking_WEB.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Entities.UserRole>
    {
        public void Configure(EntityTypeBuilder<Entities.UserRole> builder)
        {
            builder.HasData(
                new Entities.UserRole
                {
                    Id = "SelfRegistered",
                    Description = "Self-registered user",
                    CanCreate = false,
                    CanRead = false,
                    CanUpdate = false,
                    CanDelete = false
                },
                new Entities.UserRole
                {
                    Id = "Employee",
                    Description = "Company's employee",
                    CanCreate = true,
                    CanRead = true,
                    CanUpdate = false,
                    CanDelete = false
                },
                new Entities.UserRole
                {
                    Id = "Moderator",
                    Description = "Content editor",
                    CanCreate = false,
                    CanRead = true,
                    CanUpdate = true,
                    CanDelete = true
                },
                new Entities.UserRole
                {
                    Id = "Administrator",
                    Description = "System administrator",
                    CanCreate = true,
                    CanRead = true,
                    CanUpdate = true,
                    CanDelete = true
                }
            );
        }
    }
}
