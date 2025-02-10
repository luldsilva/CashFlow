using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses
{
    public interface IExepensesRepository
    {
        Task Add(Expense expense);
        Task<List<Expense>> GetAll();
        Task<Expense?> GetById(long id);
    }
}
