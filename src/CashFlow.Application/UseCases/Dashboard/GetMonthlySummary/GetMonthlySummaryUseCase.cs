using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Repositories.CreditCardStatements;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Repositories.MonthlyClosures;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;
using CashFlow.Application.UseCases.CreditCardStatements;

namespace CashFlow.Application.UseCases.Dashboard.GetMonthlySummary
{
    public class GetMonthlySummaryUseCase : IGetMonthlySummaryUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly IFinancialObligationsReadOnlyRepository _financialObligationsRepository;
        private readonly ICreditCardStatementsReadOnlyRepository _creditCardStatementsRepository;
        private readonly IMonthlyClosuresReadOnlyRepository _monthlyClosuresRepository;
        private readonly ILoggedUser _loggedUser;

        public GetMonthlySummaryUseCase(
            IHouseholdReadOnlyRepository householdRepository,
            IFinancialObligationsReadOnlyRepository financialObligationsRepository,
            ICreditCardStatementsReadOnlyRepository creditCardStatementsRepository,
            IMonthlyClosuresReadOnlyRepository monthlyClosuresRepository,
            ILoggedUser loggedUser)
        {
            _householdRepository = householdRepository;
            _financialObligationsRepository = financialObligationsRepository;
            _creditCardStatementsRepository = creditCardStatementsRepository;
            _monthlyClosuresRepository = monthlyClosuresRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseMonthlySummary> Execute(DateTime competenceDate)
        {
            var normalizedCompetenceDate = NormalizeCompetenceDate(competenceDate);
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before viewing the monthly summary.");
            }

            var obligations = await _financialObligationsRepository.GetAllByCompetence(household.Id, normalizedCompetenceDate);
            var creditCardStatements = await _creditCardStatementsRepository.GetByMonth(household.Id, normalizedCompetenceDate);
            var closure = await _monthlyClosuresRepository.GetByMonth(household.Id, normalizedCompetenceDate);

            var plannedIncome = household.IncomeSources.Sum(source => source.Amount);
            var obligationsPaidOutflow = obligations
                .Where(obligation => obligation.Status == FinancialObligationStatus.Paid)
                .Sum(obligation => obligation.PaidAmount ?? obligation.Amount);
            var obligationsCommittedOutflow = obligations
                .Where(obligation => obligation.Status != FinancialObligationStatus.Paid)
                .Sum(obligation => obligation.Amount);
            var creditCardPaidOutflow = creditCardStatements
                .Where(statement => statement.Status == Domain.Enums.CreditCardStatementStatus.Paid)
                .Sum(statement => statement.PaidAmount ?? statement.TotalAmount);
            var creditCardCommittedOutflow = creditCardStatements
                .Where(statement => statement.Status != Domain.Enums.CreditCardStatementStatus.Paid)
                .Sum(statement => statement.TotalAmount);
            var paidOutflow = obligationsPaidOutflow + creditCardPaidOutflow;
            var committedOutflow = obligationsCommittedOutflow + creditCardCommittedOutflow;
            var freeToSpend = plannedIncome - paidOutflow - committedOutflow;
            var freeToInvest = CalculateFreeToInvest(household, obligations, plannedIncome, freeToSpend);
            var commitmentPercentage = plannedIncome <= 0
                ? 0
                : Math.Round(((paidOutflow + committedOutflow) / plannedIncome) * 100m, 2);
            var remainingToAllocate = Math.Max(freeToSpend, 0);

            return new ResponseMonthlySummary
            {
                CompetenceDate = normalizedCompetenceDate,
                PlannedIncome = plannedIncome,
                PaidOutflow = paidOutflow,
                CommittedOutflow = committedOutflow,
                CommitmentPercentage = commitmentPercentage,
                FreeToSpend = freeToSpend,
                FreeToInvest = freeToInvest,
                RemainingToAllocate = remainingToAllocate,
                CreditCardPaidOutflow = creditCardPaidOutflow,
                CreditCardCommittedOutflow = creditCardCommittedOutflow,
                IsMonthClosed = closure is not null,
                Buckets = BuildBucketSummary(household, obligations, plannedIncome),
                SuggestedBuckets = BuildSuggestedBuckets(household, remainingToAllocate),
                CreditCardStatements = creditCardStatements
                    .Select(CreditCardStatementResponseMapper.Map)
                    .ToList(),
                UpcomingObligations = obligations
                    .Where(obligation => obligation.Status != FinancialObligationStatus.Paid)
                    .OrderBy(obligation => obligation.DueDate)
                    .ThenBy(obligation => obligation.Title)
                    .Select(obligation => new ResponseUpcomingObligation
                    {
                        Id = obligation.Id,
                        Title = obligation.Title,
                        CategoryId = obligation.ExpenseCategoryId,
                        CategoryName = obligation.CategoryName,
                        BucketCode = obligation.BucketCode,
                        RequiresReview = obligation.RecurrenceType == FinancialObligationRecurrenceType.VariableMonthly
                            || string.IsNullOrWhiteSpace(obligation.BucketCode),
                        Amount = obligation.Amount,
                        DueDate = obligation.DueDate,
                        Status = (CashFlow.Communication.Enums.FinancialObligationStatus)obligation.Status
                    })
                    .ToList()
            };
        }

        private static List<ResponseMonthlyBucketSummary> BuildBucketSummary(Household household, List<FinancialObligation> obligations, decimal plannedIncome)
        {
            return household.PlanningBuckets
                .Where(bucket => bucket.IsActive)
                .OrderBy(bucket => bucket.DisplayOrder)
                .Select(bucket =>
                {
                    var bucketPaid = obligations
                        .Where(obligation => obligation.BucketCode.Equals(bucket.Code, StringComparison.OrdinalIgnoreCase)
                            && obligation.Status == FinancialObligationStatus.Paid)
                        .Sum(obligation => obligation.PaidAmount ?? obligation.Amount);

                    var bucketCommitted = obligations
                        .Where(obligation => obligation.BucketCode.Equals(bucket.Code, StringComparison.OrdinalIgnoreCase)
                            && obligation.Status != FinancialObligationStatus.Paid)
                        .Sum(obligation => obligation.Amount);

                    var plannedAmount = Math.Round(plannedIncome * bucket.Percentage / 100m, 2);

                    return new ResponseMonthlyBucketSummary
                    {
                        BucketCode = bucket.Code,
                        BucketName = bucket.Name,
                        PlannedAmount = plannedAmount,
                        PaidAmount = bucketPaid,
                        CommittedAmount = bucketCommitted,
                        RemainingAmount = plannedAmount - bucketPaid - bucketCommitted
                    };
                })
                .ToList();
        }

        private static decimal CalculateFreeToInvest(
            Household household,
            List<FinancialObligation> obligations,
            decimal plannedIncome,
            decimal freeToSpend)
        {
            var investmentBucket = household.PlanningBuckets
                .FirstOrDefault(bucket => bucket.IsActive && bucket.Code.Equals("investments", StringComparison.OrdinalIgnoreCase));

            if (investmentBucket is null)
            {
                return Math.Max(freeToSpend, 0);
            }

            var plannedInvestmentAmount = Math.Round(plannedIncome * investmentBucket.Percentage / 100m, 2);
            var alreadyAllocatedToInvestments = obligations
                .Where(obligation => obligation.BucketCode.Equals("investments", StringComparison.OrdinalIgnoreCase))
                .Sum(obligation => obligation.Status == FinancialObligationStatus.Paid
                    ? obligation.PaidAmount ?? obligation.Amount
                    : obligation.Amount);

            var remainingInvestmentTarget = Math.Max(plannedInvestmentAmount - alreadyAllocatedToInvestments, 0);

            return Math.Min(Math.Max(freeToSpend, 0), remainingInvestmentTarget);
        }

        private static List<ResponseSuggestedPlanningBucket> BuildSuggestedBuckets(Household household, decimal remainingToAllocate)
        {
            if (household.PlanningBuckets.Any(bucket => bucket.IsActive))
            {
                return [];
            }

            var suggestions = new[]
            {
                new { Code = "free", Name = "Livre", Percentage = 50m },
                new { Code = "investments", Name = "Investimentos", Percentage = 30m },
                new { Code = "reserve", Name = "Reserva", Percentage = 20m }
            };

            return suggestions
                .Select(suggestion => new ResponseSuggestedPlanningBucket
                {
                    Code = suggestion.Code,
                    Name = suggestion.Name,
                    SuggestedPercentage = suggestion.Percentage,
                    SuggestedAmount = Math.Round(remainingToAllocate * suggestion.Percentage / 100m, 2)
                })
                .ToList();
        }

        private static DateTime NormalizeCompetenceDate(DateTime competenceDate)
        {
            return new DateTime(competenceDate.Year, competenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
