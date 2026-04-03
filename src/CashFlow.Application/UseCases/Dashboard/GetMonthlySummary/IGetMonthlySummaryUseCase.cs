using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.Dashboard.GetMonthlySummary
{
    public interface IGetMonthlySummaryUseCase
    {
        Task<ResponseMonthlySummary> Execute(DateTime competenceDate);
    }
}
