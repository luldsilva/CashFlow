using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.MonthlyReview.Prepare
{
    public interface IPrepareMonthUseCase
    {
        Task<ResponsePreparedMonth> Execute(RequestPrepareMonth request);
    }
}
