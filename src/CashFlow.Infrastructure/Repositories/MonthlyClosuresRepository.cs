using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.MonthlyClosures;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    internal class MonthlyClosuresRepository : IMonthlyClosuresReadOnlyRepository, IMonthlyClosuresWriteOnlyRepository, IMonthlyClosuresUpdateOnlyRepository
    {
        private readonly CashFlowDbContext _dbContext;

        public MonthlyClosuresRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(MonthlyClosure closure)
        {
            await _dbContext.MonthlyClosures.AddAsync(closure);
        }

        async Task<MonthlyClosure?> IMonthlyClosuresReadOnlyRepository.GetByMonth(long householdId, DateTime competenceDate)
        {
            return await _dbContext.MonthlyClosures
                .AsNoTracking()
                .FirstOrDefaultAsync(closure => closure.HouseholdId == householdId && closure.CompetenceDate == competenceDate);
        }

        async Task<MonthlyClosure?> IMonthlyClosuresUpdateOnlyRepository.GetByMonth(long householdId, DateTime competenceDate)
        {
            return await _dbContext.MonthlyClosures
                .FirstOrDefaultAsync(closure => closure.HouseholdId == householdId && closure.CompetenceDate == competenceDate);
        }

        public void Update(MonthlyClosure closure)
        {
            _dbContext.MonthlyClosures.Update(closure);
        }
    }
}
