using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.MonthlyReview.Close
{
    public interface ICloseMonthUseCase
    {
        Task<ResponseMonthlyClosure> Execute(RequestCloseMonth request);
    }
}
