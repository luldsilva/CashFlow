using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.PasswordResetTokens;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    internal class PasswordResetTokensRepository : IPasswordResetTokensReadOnlyRepository, IPasswordResetTokensWriteOnlyRepository, IPasswordResetTokensUpdateOnlyRepository
    {
        private readonly CashFlowDbContext _dbContext;

        public PasswordResetTokensRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(PasswordResetToken token)
        {
            await _dbContext.PasswordResetTokens.AddAsync(token);
        }

        async Task<PasswordResetToken?> IPasswordResetTokensReadOnlyRepository.GetActiveByTokenHash(string tokenHash)
        {
            return await _dbContext.PasswordResetTokens
                .AsNoTracking()
                .Include(token => token.User)
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash && token.UsedAt == null && token.ExpiresAt > DateTime.UtcNow);
        }

        async Task<PasswordResetToken?> IPasswordResetTokensUpdateOnlyRepository.GetActiveByTokenHash(string tokenHash)
        {
            return await _dbContext.PasswordResetTokens
                .Include(token => token.User)
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash && token.UsedAt == null && token.ExpiresAt > DateTime.UtcNow);
        }

        public async Task InvalidateActiveTokens(long userId)
        {
            var activeTokens = await _dbContext.PasswordResetTokens
                .Where(token => token.UserId == userId && token.UsedAt == null && token.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in activeTokens)
            {
                token.UsedAt = DateTime.UtcNow;
            }
        }

        public void Update(PasswordResetToken token)
        {
            _dbContext.PasswordResetTokens.Update(token);
        }
    }
}
