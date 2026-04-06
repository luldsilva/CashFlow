namespace CashFlow.Domain.Entities
{
    public class PasswordResetToken
    {
        public long Id { get; set; }
        public string TokenHash { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UsedAt { get; set; }

        public long UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
