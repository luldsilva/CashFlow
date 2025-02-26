using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Entities
{
    public class Expense
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public PaymentType PaymentType { get; set; }

        public long UserId { get; set; }
        public User User { get; set; } = default!;
    }
}
