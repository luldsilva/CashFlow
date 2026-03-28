using CashFlow.Communication.Responses;
using System.IO;

namespace CashFlow.Application.UseCases.Expenses.Attachments
{
    public interface IAddExpenseAttachmentUseCase
    {
        Task<ResponseExpenseAttachment> Execute(
            long expenseId,
            Stream content,
            string fileName,
            string contentType,
            long sizeInBytes,
            CancellationToken cancellationToken = default);
    }
}
