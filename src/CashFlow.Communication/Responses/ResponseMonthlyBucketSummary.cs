namespace CashFlow.Communication.Responses
{
    public class ResponseMonthlyBucketSummary
    {
        public string BucketCode { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public decimal PlannedAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
    }
}
