namespace CashFlow.Communication.Requests
{
    public class RequestExpenseCategory
    {
        public string Name { get; set; } = string.Empty;
        public string? BucketName { get; set; }
    }
}
