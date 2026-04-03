using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.CreditCardStatements;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    internal class CreditCardStatementsRepository : ICreditCardStatementsReadOnlyRepository, ICreditCardStatementsWriteOnlyRepository
    {
        private readonly CashFlowDbContext _dbContext;

        public CreditCardStatementsRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(CreditCardStatement statement)
        {
            await _dbContext.CreditCardStatements.AddAsync(statement);
        }

        public async Task<List<CreditCardStatement>> GetByMonth(long householdId, DateTime competenceDate)
        {
            return await _dbContext.CreditCardStatements
                .AsNoTracking()
                .Include(statement => statement.CreditCard)
                .Where(statement => statement.CreditCard.HouseholdId == householdId && statement.CompetenceDate == competenceDate)
                .OrderBy(statement => statement.DueDate)
                .ThenBy(statement => statement.CreditCard.Name)
                .ToListAsync();
        }
    }
}
