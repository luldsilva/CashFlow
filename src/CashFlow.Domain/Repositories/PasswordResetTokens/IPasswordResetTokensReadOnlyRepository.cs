using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.PasswordResetTokens
{
    public interface IPasswordResetTokensReadOnlyRepository
    {
        Task<PasswordResetToken?> GetActiveByTokenHash(string tokenHash);
    }
}
