using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.FinancialSetup.Upsert
{
    public interface IUpsertFinancialSetupUseCase
    {
        Task<ResponseFinancialSetup> Execute(RequestUpsertFinancialSetup request);
    }
}
