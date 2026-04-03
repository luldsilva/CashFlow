using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.CreditCards;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    internal class CreditCardsRepository : ICreditCardsReadOnlyRepository, ICreditCardsWriteOnlyRepository
    {
        private readonly CashFlowDbContext _dbContext;

        public CreditCardsRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(CreditCard creditCard)
        {
            await _dbContext.CreditCards.AddAsync(creditCard);
        }

        public async Task<List<CreditCard>> GetAll(long householdId)
        {
            return await _dbContext.CreditCards
                .AsNoTracking()
                .Where(card => card.HouseholdId == householdId)
                .OrderBy(card => card.Name)
                .ToListAsync();
        }

        public async Task<CreditCard?> GetById(long householdId, long id)
        {
            return await _dbContext.CreditCards
                .AsNoTracking()
                .FirstOrDefaultAsync(card => card.HouseholdId == householdId && card.Id == id);
        }
    }
}
