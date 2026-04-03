using CashFlow.Application.UseCases.Dashboard.GetMonthlySummary;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Repositories.MonthlyClosures;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.MonthlyReview.Close
{
    public class CloseMonthUseCase : ICloseMonthUseCase
    {
        private readonly IGetMonthlySummaryUseCase _monthlySummaryUseCase;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly IMonthlyClosuresWriteOnlyRepository _writeOnlyRepository;
        private readonly IMonthlyClosuresUpdateOnlyRepository _updateOnlyRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public CloseMonthUseCase(
            IGetMonthlySummaryUseCase monthlySummaryUseCase,
            IHouseholdReadOnlyRepository householdRepository,
            IMonthlyClosuresWriteOnlyRepository writeOnlyRepository,
            IMonthlyClosuresUpdateOnlyRepository updateOnlyRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _monthlySummaryUseCase = monthlySummaryUseCase;
            _householdRepository = householdRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _updateOnlyRepository = updateOnlyRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseMonthlyClosure> Execute(RequestCloseMonth request)
        {
            var competenceDate = NormalizeCompetenceDate(request.CompetenceDate);
            var summary = await _monthlySummaryUseCase.Execute(competenceDate);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before closing the month.");
            }

            var closure = await _updateOnlyRepository.GetByMonth(household.Id, competenceDate);
            var isNewClosure = closure is null;

            if (isNewClosure)
            {
                closure = new MonthlyClosure
                {
                    HouseholdId = household.Id,
                    CompetenceDate = competenceDate
                };
                await _writeOnlyRepository.Add(closure);
            }

            closure!.PlannedIncome = summary.PlannedIncome;
            closure.PaidOutflow = summary.PaidOutflow;
            closure.CommittedOutflow = summary.CommittedOutflow;
            closure.FreeToSpend = summary.FreeToSpend;
            closure.FreeToInvest = summary.FreeToInvest;
            closure.Notes = request.Notes?.Trim();
            closure.ClosedAt = DateTime.UtcNow;

            if (!isNewClosure)
            {
                _updateOnlyRepository.Update(closure);
            }

            await _unitOfWork.Commit();

            return new ResponseMonthlyClosure
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
            };
        }

        private static DateTime NormalizeCompetenceDate(DateTime competenceDate)
        {
            return new DateTime(competenceDate.Year, competenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
