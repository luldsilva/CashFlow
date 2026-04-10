using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Responses
{
    public class ResponseCreditCardStatement
    {
        public long Id { get; set; }
        public long CreditCardId { get; set; }
        public string CreditCardName { get; set; } = string.Empty;
        public DateTime CompetenceDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public CreditCardStatementStatus Status { get; set; }
        public bool RequiresReview { get; set; }
    }
}
