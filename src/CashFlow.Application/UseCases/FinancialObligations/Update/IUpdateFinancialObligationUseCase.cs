using CashFlow.Communication.Requests;

namespace CashFlow.Application.UseCases.FinancialObligations.Update
{
    public interface IUpdateFinancialObligationUseCase
    {
        Task Execute(long id, RequestFinancialObligation request);
    }
}
