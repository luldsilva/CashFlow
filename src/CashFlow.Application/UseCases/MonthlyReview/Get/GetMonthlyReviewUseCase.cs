using CashFlow.Application.UseCases.Dashboard.GetMonthlySummary;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Repositories.MonthlyClosures;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.MonthlyReview.Get
{
    public class GetMonthlyReviewUseCase : IGetMonthlyReviewUseCase
    {
        private readonly IGetMonthlySummaryUseCase _monthlySummaryUseCase;
        private readonly IMonthlyClosuresReadOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;

        public GetMonthlyReviewUseCase(
            IGetMonthlySummaryUseCase monthlySummaryUseCase,
            IMonthlyClosuresReadOnlyRepository repository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser)
        {
            _monthlySummaryUseCase = monthlySummaryUseCase;
            _repository = repository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseMonthlyReview> Execute(DateTime competenceDate)
        {
            var normalizedCompetenceDate = new DateTime(competenceDate.Year, competenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var summary = await _monthlySummaryUseCase.Execute(normalizedCompetenceDate);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before reviewing the month.");
            }

            var closure = await _repository.GetByMonth(household.Id, normalizedCompetenceDate);

            return new ResponseMonthlyReview
            {
                CompetenceDate = normalizedCompetenceDate,
                Summary = summary,
                Closure = closure is null ? null : new ResponseMonthlyClosure
                {
                    Id = closure.Id,
                    CompetenceDate = closure.CompetenceDate,
                    PlannedIncome = closure.PlannedIncome,
                    PaidOutflow = closure.PaidOutflow,
                    CommittedOutflow = closure.CommittedOutflow,
                    FreeToSpend = closure.FreeToSpend,
                    FreeToInvest = closure.FreeToInvest,
                    Notes = closure.Notes,
                    ClosedAt = closure.ClosedAt
                }
            };
        }
    }
}
