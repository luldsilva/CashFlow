namespace CashFlow.Infrastructure.Storage
{
    public class StorageSettings
    {
        public string Provider { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string PublicEndpoint { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
