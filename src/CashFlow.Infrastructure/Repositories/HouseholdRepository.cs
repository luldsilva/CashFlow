using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.Repositories
{
    internal class HouseholdRepository : IHouseholdReadOnlyRepository, IHouseholdWriteOnlyRepository
    {
        private readonly CashFlowDbContext _dbContext;

        public HouseholdRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Add(Household household)
        {
            await _dbContext.Households.AddAsync(household);
        }

        public async Task Delete(long id)
        {
            var household = await _dbContext.Households.FirstOrDefaultAsync(item => item.Id == id);

            if (household is not null)
            {
                _dbContext.Households.Remove(household);
            }
        }

        public async Task<Household?> GetByUserId(long userId)
        {
            return await _dbContext.Households
                .Include(household => household.IncomeSources)
                .Include(household => household.FinancialObligations)
                .Include(household => household.PlanningBuckets)
                .Include(household => household.ExpenseCategories)
                .FirstOrDefaultAsync(household => household.UserId == userId);
        }
    }
}
