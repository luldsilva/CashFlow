using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Expenses.Register
{
    public class RegisterExpenseUseCase
    {
        public ResponseRegisteredExpense Execute(RequestRegisterExpense request)
        {
            return new ResponseRegisteredExpense();
        }
    }
}
