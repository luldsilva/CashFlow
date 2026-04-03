using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    internal class FinancialObligationsRepository : IFinancialObligationsReadOnlyRepository, IFinancialObligationsWriteOnlyRepository, IFinancialObligationsUpdateOnlyRepository
    {
        private readonly CashFlowDbContext _dbContext;

        public FinancialObligationsRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(FinancialObligation obligation)
        {
            await _dbContext.FinancialObligations.AddAsync(obligation);
        }

        public async Task Delete(long id)
        {
            var obligation = await _dbContext.FinancialObligations.FindAsync(id);

            if (obligation is not null)
            {
                _dbContext.FinancialObligations.Remove(obligation);
            }
        }

        public async Task<List<FinancialObligation>> GetAllByCompetence(long householdId, DateTime competenceDate)
        {
            return await _dbContext.FinancialObligations
                .AsNoTracking()
                .Where(obligation => obligation.HouseholdId == householdId && obligation.CompetenceDate == competenceDate)
                .OrderBy(obligation => obligation.DueDate)
                .ThenBy(obligation => obligation.Title)
                .ToListAsync();
        }

        async Task<FinancialObligation?> IFinancialObligationsReadOnlyRepository.GetById(long householdId, long id)
        {
            return await _dbContext.FinancialObligations
                .AsNoTracking()
                .FirstOrDefaultAsync(obligation => obligation.HouseholdId == householdId && obligation.Id == id);
        }

        async Task<FinancialObligation?> IFinancialObligationsUpdateOnlyRepository.GetById(long householdId, long id)
        {
            return await _dbContext.FinancialObligations
                .FirstOrDefaultAsync(obligation => obligation.HouseholdId == householdId && obligation.Id == id);
        }

        public void Update(FinancialObligation obligation)
        {
            _dbContext.FinancialObligations.Update(obligation);
        }
    }
}
