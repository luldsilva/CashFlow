using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.ExpenseCategories.GetAll
{
    public interface IGetAllExpenseCategoriesUseCase
    {
        Task<ResponseExpenseCategories> Execute();
    }
}
