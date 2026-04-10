using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.FinancialSetup.Register
{
    public interface IRegisterFinancialSetupUseCase
    {
        Task<ResponseFinancialSetup> Execute(RequestUpsertFinancialSetup request);
    }
}
