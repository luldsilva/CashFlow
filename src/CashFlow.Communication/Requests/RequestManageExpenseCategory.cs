namespace CashFlow.Communication.Requests
{
    public class RequestManageExpenseCategory
    {
        public string Name { get; set; } = string.Empty;
        public string? BucketCode { get; set; }
    }
}
