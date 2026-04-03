using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.FinancialObligations.Register
{
    public interface IRegisterFinancialObligationUseCase
    {
        Task<ResponseFinancialObligation> Execute(RequestFinancialObligation request);
    }
}
