using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBudget.DAL.Entities.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.IsBlocked)
            .HasDefaultValue(false);

        builder.HasMany(u => u.Expenses)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}