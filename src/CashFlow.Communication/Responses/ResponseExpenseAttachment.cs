namespace CashFlow.Communication.Responses
{
    public class ResponseExpenseAttachment
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
