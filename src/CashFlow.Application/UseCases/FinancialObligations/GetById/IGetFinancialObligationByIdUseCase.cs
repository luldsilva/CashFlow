using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.FinancialObligations.GetById
{
    public interface IGetFinancialObligationByIdUseCase
    {
        Task<ResponseFinancialObligation> Execute(long id);
    }
}
