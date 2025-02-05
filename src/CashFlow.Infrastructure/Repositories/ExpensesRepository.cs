using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Infrastructure.DataAccess;

namespace CashFlow.Infrastructure.Repositories
{
    internal class ExpensesRepository : IExepensesRepository
    {
        private readonly CashFlowDbContext _dbContext;
        public ExpensesRepository(CashFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(Expense expense)
        {           
            _dbContext.Expenses.Add(expense);
        }
    }
}
