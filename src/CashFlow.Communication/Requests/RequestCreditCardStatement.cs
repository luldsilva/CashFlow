using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Requests
{
    public class RequestCreditCardStatement
    {
        public long CreditCardId { get; set; }
        public DateTime CompetenceDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? PaidDate { get; set; }
        public CreditCardStatementStatus Status { get; set; }
    }
}
