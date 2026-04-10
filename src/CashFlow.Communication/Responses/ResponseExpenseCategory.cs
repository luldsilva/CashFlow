namespace CashFlow.Communication.Responses
{
    public class ResponseExpenseCategory
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? BucketName { get; set; }
        public string? BucketCode { get; set; }
    }
}
