namespace CashFlow.Communication.Responses
{
    public class ResponseMonthlySummary
    {
        public DateTime CompetenceDate { get; set; }
        public decimal PlannedIncome { get; set; }
        public decimal PaidOutflow { get; set; }
        public decimal CommittedOutflow { get; set; }
        public decimal CommitmentPercentage { get; set; }
        public decimal FreeToSpend { get; set; }
        public decimal FreeToInvest { get; set; }
        public decimal RemainingToAllocate { get; set; }
        public decimal CreditCardPaidOutflow { get; set; }
        public decimal CreditCardCommittedOutflow { get; set; }
        public bool IsMonthClosed { get; set; }
        public List<ResponseMonthlyBucketSummary> Buckets { get; set; } = [];
        public List<ResponseSuggestedPlanningBucket> SuggestedBuckets { get; set; } = [];
        public List<ResponseCreditCardStatement> CreditCardStatements { get; set; } = [];
        public List<ResponseUpcomingObligation> UpcomingObligations { get; set; } = [];
    }
}
