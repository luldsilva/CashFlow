using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Domain.Services.Storage;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using System.IO;

namespace CashFlow.Application.UseCases.Expenses.Attachments
{
    public class AddExpenseAttachmentUseCase : IAddExpenseAttachmentUseCase
    {
        private static readonly HashSet<string> AllowedContentTypes =
        [
            "image/jpeg",
            "image/png",
            "image/webp",
            "application/pdf"
        ];

        private const long MaxFileSizeInBytes = 10 * 1024 * 1024;

        private readonly IExpensesUpdateOnlyrepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggedUser _loggedUser;
        private readonly IFileStorageService _fileStorageService;

        public AddExpenseAttachmentUseCase(
            IExpensesUpdateOnlyrepository repository,
            IUnitOfWork unitOfWork,
            ILoggedUser loggedUser,
            IFileStorageService fileStorageService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _loggedUser = loggedUser;
            _fileStorageService = fileStorageService;
        }

        public async Task<ResponseExpenseAttachment> Execute(
            long expenseId,
            Stream content,
            string fileName,
            string contentType,
            long sizeInBytes,
            CancellationToken cancellationToken = default)
        {
            Validate(fileName, contentType, sizeInBytes);

            var loggedUser = await _loggedUser.Get();
            var expense = await _repository.GetById(loggedUser, expenseId);

            if (expense is null)
            {
                throw new NotFoundException(ResourceErrorMessages.EXPENSE_NOT_FOUND);
            }

            var attachment = new ExpenseAttachment
            {
                FileName = fileName.Trim(),
                ContentType = contentType,
                SizeInBytes = sizeInBytes,
                UploadedAt = DateTime.UtcNow,
                StorageKey = CreateStorageKey(expenseId, fileName)
            };

            await _fileStorageService.StoreAsync(new StorageFile
            {
                FileName = attachment.FileName,
                ContentType = attachment.ContentType,
                StorageKey = attachment.StorageKey,
                Content = content
            }, cancellationToken);

            expense.Attachments.Add(attachment);
            _repository.Update(expense);

            await _unitOfWork.Commit();

            return new ResponseExpenseAttachment
            {
                Id = attachment.Id,
                FileName = attachment.FileName,
                ContentType = attachment.ContentType,
                SizeInBytes = attachment.SizeInBytes,
                UploadedAt = attachment.UploadedAt
            };
        }

        private static void Validate(string fileName, string contentType, long sizeInBytes)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(fileName))
            {
                errors.Add(ResourceErrorMessages.ATTACHMENT_FILE_NAME_REQUIRED);
            }

            if (sizeInBytes <= 0)
            {
                errors.Add(ResourceErrorMessages.ATTACHMENT_EMPTY);
            }

            if (sizeInBytes > MaxFileSizeInBytes)
            {
                errors.Add(ResourceErrorMessages.ATTACHMENT_TOO_LARGE);
            }

            if (string.IsNullOrWhiteSpace(contentType) || !AllowedContentTypes.Contains(contentType))
            {
                errors.Add(ResourceErrorMessages.ATTACHMENT_CONTENT_TYPE_NOT_ALLOWED);
            }

            if (errors.Count > 0)
            {
                throw new ErrorOnValidationException(errors);
            }
        }

        private static string CreateStorageKey(long expenseId, string fileName)
        {
            var extension = Path.GetExtension(fileName);
            var normalizedExtension = string.IsNullOrWhiteSpace(extension) ? string.Empty : extension.ToLowerInvariant();

            return $"expenses/{expenseId}/{Guid.NewGuid():N}{normalizedExtension}";
        }
    }
}
