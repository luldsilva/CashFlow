using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses
{
    public interface IExpensesWriteOnlyrepository
    {
        Task Add(Expense expense);
        Task Delete(long id);
    }
}
