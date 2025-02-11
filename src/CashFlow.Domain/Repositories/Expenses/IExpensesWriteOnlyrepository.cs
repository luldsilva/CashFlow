using CashFlow.Domain.Entities;

namespace CashFlow.Domain.Repositories.Expenses
{
    public interface IExpensesWriteOnlyrepository
    {
        Task Add(Expense expense);
        /// <summary>
        /// Essa funcao retorna TRUE se a delecao tiver sucesso
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Delete(long id);
    }
}
