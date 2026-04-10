using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.ExpenseCategories.Update
{
    public interface IUpdateExpenseCategoryUseCase
    {
        Task<ResponseExpenseCategory> Execute(long id, RequestManageExpenseCategory request);
    }
}
