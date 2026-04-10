using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.ExpenseCategories.Register
{
    public interface IRegisterExpenseCategoryUseCase
    {
        Task<ResponseExpenseCategory> Execute(RequestManageExpenseCategory request);
    }
}
