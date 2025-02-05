using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses
{
    public interface IExepensesRepository
    {
        void Add(Expense expense);
    }
}
