using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Infrastructure.DataAccess;

namespace CashFlow.Infrastructure.Repositories
{
    internal class ExpensesRepository : IExepensesRepository
    {
        public void Add(Expense expense)
        {
            var dbContext = new CashFlowDbContext();
            
            dbContext.Expenses.Add(expense);

            dbContext.SaveChanges();
        }
    }
}
