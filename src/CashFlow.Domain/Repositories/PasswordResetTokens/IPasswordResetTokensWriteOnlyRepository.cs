using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.PasswordResetTokens
{
    public interface IPasswordResetTokensWriteOnlyRepository
    {
        Task Add(PasswordResetToken token);
    }
}
