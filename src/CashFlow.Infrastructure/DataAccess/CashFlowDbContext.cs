using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess
{
    public class CashFlowDbContext : DbContext
    {
        public CashFlowDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseAttachment> ExpenseAttachments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.HasMany(expense => expense.Attachments)
                    .WithOne(attachment => attachment.Expense)
                    .HasForeignKey(attachment => attachment.ExpenseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExpenseAttachment>(entity =>
            {
                entity.Property(attachment => attachment.FileName).HasMaxLength(255);
                entity.Property(attachment => attachment.ContentType).HasMaxLength(255);
                entity.Property(attachment => attachment.StorageKey).HasMaxLength(512);
                entity.HasIndex(attachment => attachment.ExpenseId);
            });
        }
    }
}
