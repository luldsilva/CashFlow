namespace CashFlow.Communication.Responses
{
    public class ResponseMonthlyReview
    {
        public DateTime CompetenceDate { get; set; }
        public ResponseMonthlySummary Summary { get; set; } = new();
        public ResponseMonthlyClosure? Closure { get; set; }
    }
}
