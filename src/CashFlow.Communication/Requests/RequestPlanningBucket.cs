namespace CashFlow.Communication.Requests
{
    public class RequestPlanningBucket
    {
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
