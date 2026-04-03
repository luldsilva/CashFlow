namespace CashFlow.Communication.Responses
{
    public class ResponseExpenseCategory
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BucketCode { get; set; } = string.Empty;
    }
}
