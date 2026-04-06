using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess
{
    public class CashFlowDbContext : DbContext
    {
        public CashFlowDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseAttachment> ExpenseAttachments { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<CreditCardStatement> CreditCardStatements { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<FinancialObligation> FinancialObligations { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<IncomeSource> IncomeSources { get; set; }
        public DbSet<MonthlyClosure> MonthlyClosures { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<PlanningBucket> PlanningBuckets { get; set; }
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

            modelBuilder.Entity<Household>(entity =>
            {
                entity.Property(household => household.Name).HasMaxLength(150);
                entity.HasIndex(household => household.UserId).IsUnique();
                entity.HasMany(household => household.CreditCards)
                    .WithOne(creditCard => creditCard.Household)
                    .HasForeignKey(creditCard => creditCard.HouseholdId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(household => household.IncomeSources)
                    .WithOne(source => source.Household)
                    .HasForeignKey(source => source.HouseholdId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(household => household.FinancialObligations)
                    .WithOne(obligation => obligation.Household)
                    .HasForeignKey(obligation => obligation.HouseholdId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(household => household.PlanningBuckets)
                    .WithOne(bucket => bucket.Household)
                    .HasForeignKey(bucket => bucket.HouseholdId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(household => household.ExpenseCategories)
                    .WithOne(category => category.Household)
                    .HasForeignKey(category => category.HouseholdId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(household => household.MonthlyClosures)
                    .WithOne(closure => closure.Household)
                    .HasForeignKey(closure => closure.HouseholdId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CreditCard>(entity =>
            {
                entity.Property(creditCard => creditCard.Name).HasMaxLength(100);
                entity.Property(creditCard => creditCard.Brand).HasMaxLength(50);
                entity.Property(creditCard => creditCard.LastFourDigits).HasMaxLength(4);
                entity.HasIndex(creditCard => creditCard.HouseholdId);
                entity.HasMany(creditCard => creditCard.Statements)
                    .WithOne(statement => statement.CreditCard)
                    .HasForeignKey(statement => statement.CreditCardId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CreditCardStatement>(entity =>
            {
                entity.Property(statement => statement.TotalAmount).HasPrecision(18, 2);
                entity.Property(statement => statement.PaidAmount).HasPrecision(18, 2);
                entity.HasIndex(statement => new { statement.CreditCardId, statement.CompetenceDate }).IsUnique();
            });

            modelBuilder.Entity<IncomeSource>(entity =>
            {
                entity.Property(source => source.Name).HasMaxLength(150);
                entity.Property(source => source.Amount).HasPrecision(18, 2);
                entity.HasIndex(source => source.HouseholdId);
            });

            modelBuilder.Entity<PlanningBucket>(entity =>
            {
                entity.Property(bucket => bucket.Code).HasMaxLength(50);
                entity.Property(bucket => bucket.Name).HasMaxLength(100);
                entity.Property(bucket => bucket.Percentage).HasPrecision(5, 2);
                entity.HasIndex(bucket => new { bucket.HouseholdId, bucket.Code }).IsUnique();
            });

            modelBuilder.Entity<ExpenseCategory>(entity =>
            {
                entity.Property(category => category.Name).HasMaxLength(100);
                entity.Property(category => category.BucketCode).HasMaxLength(50);
                entity.HasIndex(category => new { category.HouseholdId, category.Name }).IsUnique();
            });

            modelBuilder.Entity<FinancialObligation>(entity =>
            {
                entity.Property(obligation => obligation.Title).HasMaxLength(150);
                entity.Property(obligation => obligation.Description).HasMaxLength(500);
                entity.Property(obligation => obligation.CategoryName).HasMaxLength(100);
                entity.Property(obligation => obligation.BucketCode).HasMaxLength(50);
                entity.Property(obligation => obligation.Amount).HasPrecision(18, 2);
                entity.Property(obligation => obligation.PaidAmount).HasPrecision(18, 2);
                entity.HasIndex(obligation => new { obligation.HouseholdId, obligation.CompetenceDate });
            });

            modelBuilder.Entity<MonthlyClosure>(entity =>
            {
                entity.Property(closure => closure.PlannedIncome).HasPrecision(18, 2);
                entity.Property(closure => closure.PaidOutflow).HasPrecision(18, 2);
                entity.Property(closure => closure.CommittedOutflow).HasPrecision(18, 2);
                entity.Property(closure => closure.FreeToSpend).HasPrecision(18, 2);
                entity.Property(closure => closure.FreeToInvest).HasPrecision(18, 2);
                entity.Property(closure => closure.Notes).HasMaxLength(1000);
                entity.HasIndex(closure => new { closure.HouseholdId, closure.CompetenceDate }).IsUnique();
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.Property(token => token.TokenHash).HasMaxLength(128);
                entity.HasIndex(token => token.TokenHash).IsUnique();
                entity.HasIndex(token => new { token.UserId, token.ExpiresAt });
            });
        }
    }
}
