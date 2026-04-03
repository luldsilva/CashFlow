using CashFlow.Communication.Responses;

namespace CashFlow.Application.UseCases.MonthlyReview.Get
{
    public interface IGetMonthlyReviewUseCase
    {
        Task<ResponseMonthlyReview> Execute(DateTime competenceDate);
    }
}
