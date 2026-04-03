using CashFlow.Communication.Enums;

namespace CashFlow.Communication.Responses
{
    public class ResponseUpcomingObligation
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BucketCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public FinancialObligationStatus Status { get; set; }
    }
}
