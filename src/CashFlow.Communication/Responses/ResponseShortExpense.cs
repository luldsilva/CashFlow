namespace CashFlow.Communication.Responses
{
    public class ResponseShortExpense
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public long? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
