using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.FinancialObligations.GetAll
{
    public interface IGetAllFinancialObligationsUseCase
    {
        Task<ResponseFinancialObligations> Execute(DateTime competenceDate);
    }
}
