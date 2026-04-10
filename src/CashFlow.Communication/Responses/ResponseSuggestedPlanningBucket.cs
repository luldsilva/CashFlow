namespace CashFlow.Communication.Responses
{
    public class ResponseSuggestedPlanningBucket
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal SuggestedPercentage { get; set; }
        public decimal SuggestedAmount { get; set; }
    }
}
