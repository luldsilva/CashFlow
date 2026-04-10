namespace CashFlow.Application.UseCases.ExpenseCategories.Delete
{
    public interface IDeleteExpenseCategoryUseCase
    {
        Task Execute(long id);
    }
}
