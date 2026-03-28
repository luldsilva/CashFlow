using Amazon.S3;
using Amazon.S3.Model;
using CashFlow.Domain.Services.Storage;

namespace CashFlow.Infrastructure.Storage
{
    public class S3CompatibleFileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly StorageSettings _storageSettings;

        public S3CompatibleFileStorageService(IAmazonS3 amazonS3, StorageSettings storageSettings)
        {
            _amazonS3 = amazonS3;
            _storageSettings = storageSettings;
        }

        public async Task StoreAsync(StorageFile file, CancellationToken cancellationToken = default)
        {
            await EnsureBucketExists(cancellationToken);

            var request = new PutObjectRequest
            {
                BucketName = _storageSettings.BucketName,
                Key = file.StorageKey,
                InputStream = file.Content,
                ContentType = file.ContentType,
                AutoCloseStream = false
            };

            await _amazonS3.PutObjectAsync(request, cancellationToken);
        }

        public async Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
        {
            await _amazonS3.DeleteObjectAsync(new DeleteObjectRequest
            {
                BucketName = _storageSettings.BucketName,
                Key = storageKey
            }, cancellationToken);
        }

        private async Task EnsureBucketExists(CancellationToken cancellationToken)
        {
            var buckets = await _amazonS3.ListBucketsAsync(cancellationToken);

            if (buckets.Buckets.Any(bucket => bucket.BucketName == _storageSettings.BucketName))
            {
                return;
            }

            await _amazonS3.PutBucketAsync(new PutBucketRequest
            {
                BucketName = _storageSettings.BucketName
            }, cancellationToken);
        }
    }
}
