using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBudget.DAL.Entities.Configuration;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount)
            .IsRequired();

        builder.Property(e => e.Date)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.HasOne(e => e.User)
            .WithMany(u => u.Expenses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}