using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.PasswordResetTokens
{
    public interface IPasswordResetTokensUpdateOnlyRepository
    {
        Task InvalidateActiveTokens(long userId);
        Task<PasswordResetToken?> GetActiveByTokenHash(string tokenHash);
        void Update(PasswordResetToken token);
    }
}
