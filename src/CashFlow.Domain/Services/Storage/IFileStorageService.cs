using System.IO;

namespace CashFlow.Domain.Services.Storage
{
    public interface IFileStorageService
    {
        Task StoreAsync(StorageFile file, CancellationToken cancellationToken = default);
        Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
    }

    public class StorageFile
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string StorageKey { get; set; } = string.Empty;
        public Stream Content { get; set; } = Stream.Null;
    }
}
