namespace CashFlow.Domain.Entities
{
    public class ExpenseAttachment
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime UploadedAt { get; set; }

        public long ExpenseId { get; set; }
        public Expense Expense { get; set; } = default!;
    }
}
