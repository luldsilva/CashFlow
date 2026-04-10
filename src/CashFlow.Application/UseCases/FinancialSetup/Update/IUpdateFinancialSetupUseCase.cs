using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.FinancialSetup.Update
{
    public interface IUpdateFinancialSetupUseCase
    {
        Task<ResponseFinancialSetup> Execute(RequestUpsertFinancialSetup request);
    }
}
