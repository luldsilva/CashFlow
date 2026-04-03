using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.FinancialSetup.Get
{
    public interface IGetFinancialSetupUseCase
    {
        Task<ResponseFinancialSetup> Execute();
    }
}
