using CashFlow.Domain.Enums;

namespace CashFlow.Domain.Entities
{
    public class CreditCardStatement
    {
        public long Id { get; set; }
        public DateTime CompetenceDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public CreditCardStatementStatus Status { get; set; }

        public long CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; } = default!;
    }
}
