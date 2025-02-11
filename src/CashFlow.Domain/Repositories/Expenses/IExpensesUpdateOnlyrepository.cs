using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses
{
    public interface IExpensesUpdateOnlyrepository
    {
        Task<Expense?> GetById(long id);
        void Update(Expense expense);
    }
}
