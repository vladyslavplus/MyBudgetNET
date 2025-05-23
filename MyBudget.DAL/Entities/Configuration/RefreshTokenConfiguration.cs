using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBudget.DAL.Entities.Configuration;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refreshTokens"); 

        builder.HasKey(rt => rt.Id); 

        builder.Property(rt => rt.UserId)
            .IsRequired()
            .HasMaxLength(450); 

        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500); 

        builder.Property(rt => rt.Expires)
            .IsRequired();

        builder.Property(rt => rt.Created)
            .IsRequired();

        builder.Property(rt => rt.CreatedByIp)
            .IsRequired()
            .HasMaxLength(45); 

        builder.Property(rt => rt.RevokedByIp)
            .HasMaxLength(45);

        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);

        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens) 
            .HasForeignKey(rt => rt.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); 

        builder.Ignore(rt => rt.IsExpired);
        builder.Ignore(rt => rt.IsActive);
    }
}