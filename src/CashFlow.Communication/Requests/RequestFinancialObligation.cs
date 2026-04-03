using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Requests
{
    public class RequestFinancialObligation
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BucketCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime CompetenceDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public FinancialObligationRecurrenceType RecurrenceType { get; set; }
        public FinancialObligationStatus Status { get; set; }
    }
}
