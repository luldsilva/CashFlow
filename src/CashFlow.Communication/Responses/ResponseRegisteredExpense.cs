namespace CashFlow.Communication.Responses
{
    public class ResponseRegisteredExpense
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
