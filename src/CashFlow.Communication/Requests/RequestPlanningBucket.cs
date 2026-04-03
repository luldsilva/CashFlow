namespace CashFlow.Communication.Requests
{
    public class RequestPlanningBucket
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Percentage { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
    }
}
