using AutoMapper;
using CashFlow.Application.UseCases.CreditCardStatements;
using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.CreditCards;
using CashFlow.Domain.Repositories.CreditCardStatements;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Repositories.MonthlyClosures;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.MonthlyReview.Prepare
{
    public class PrepareMonthUseCase : IPrepareMonthUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly IFinancialObligationsReadOnlyRepository _financialObligationsReadOnlyRepository;
        private readonly IFinancialObligationsWriteOnlyRepository _financialObligationsWriteOnlyRepository;
        private readonly ICreditCardsReadOnlyRepository _creditCardsRepository;
        private readonly ICreditCardStatementsReadOnlyRepository _creditCardStatementsReadOnlyRepository;
        private readonly ICreditCardStatementsWriteOnlyRepository _creditCardStatementsWriteOnlyRepository;
        private readonly IMonthlyClosuresReadOnlyRepository _monthlyClosuresRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PrepareMonthUseCase(
            IHouseholdReadOnlyRepository householdRepository,
            IFinancialObligationsReadOnlyRepository financialObligationsReadOnlyRepository,
            IFinancialObligationsWriteOnlyRepository financialObligationsWriteOnlyRepository,
            ICreditCardsReadOnlyRepository creditCardsRepository,
            ICreditCardStatementsReadOnlyRepository creditCardStatementsReadOnlyRepository,
            ICreditCardStatementsWriteOnlyRepository creditCardStatementsWriteOnlyRepository,
            IMonthlyClosuresReadOnlyRepository monthlyClosuresRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _householdRepository = householdRepository;
            _financialObligationsReadOnlyRepository = financialObligationsReadOnlyRepository;
            _financialObligationsWriteOnlyRepository = financialObligationsWriteOnlyRepository;
            _creditCardsRepository = creditCardsRepository;
            _creditCardStatementsReadOnlyRepository = creditCardStatementsReadOnlyRepository;
            _creditCardStatementsWriteOnlyRepository = creditCardStatementsWriteOnlyRepository;
            _monthlyClosuresRepository = monthlyClosuresRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResponsePreparedMonth> Execute(RequestPrepareMonth request)
        {
            var targetCompetenceDate = NormalizeCompetenceDate(request.TargetCompetenceDate);
            var sourceCompetenceDate = NormalizeCompetenceDate(
                request.SourceCompetenceDate ?? targetCompetenceDate.AddMonths(-1));

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before preparing a month.");
            }

            var existingClosure = await _monthlyClosuresRepository.GetByMonth(household.Id, targetCompetenceDate);
            var sourceObligations = await _financialObligationsReadOnlyRepository.GetAllByCompetence(household.Id, sourceCompetenceDate);
            var targetObligations = await _financialObligationsReadOnlyRepository.GetAllByCompetence(household.Id, targetCompetenceDate);
            var sourceStatements = await _creditCardStatementsReadOnlyRepository.GetByMonth(household.Id, sourceCompetenceDate);
            var targetStatements = await _creditCardStatementsReadOnlyRepository.GetByMonth(household.Id, targetCompetenceDate);
            var creditCards = await _creditCardsRepository.GetAll(household.Id);

            var createdObligations = new List<FinancialObligation>();
            var skippedObligations = 0;

            foreach (var sourceObligation in sourceObligations.Where(IsRecurring))
            {
                if (targetObligations.Any(existing => IsSameRecurringSeed(existing, sourceObligation)))
                {
                    skippedObligations++;
                    continue;
                }

                var clonedObligation = new FinancialObligation
                {
                    HouseholdId = household.Id,
                    Title = sourceObligation.Title,
                    Description = sourceObligation.Description,
                    CategoryName = sourceObligation.CategoryName,
                    BucketCode = sourceObligation.BucketCode,
                    Amount = sourceObligation.Amount,
                    CompetenceDate = targetCompetenceDate,
                    DueDate = CopyDayToMonth(sourceObligation.DueDate, targetCompetenceDate),
                    RecurrenceType = sourceObligation.RecurrenceType,
                    Status = Domain.Enums.FinancialObligationStatus.Expected
                };

                await _financialObligationsWriteOnlyRepository.Add(clonedObligation);
                createdObligations.Add(clonedObligation);
            }

            var createdStatements = new List<CreditCardStatement>();
            var skippedStatements = 0;

            foreach (var creditCard in creditCards)
            {
                if (targetStatements.Any(statement => statement.CreditCardId == creditCard.Id))
                {
                    skippedStatements++;
                    continue;
                }

                var sourceStatement = sourceStatements
                    .FirstOrDefault(statement => statement.CreditCardId == creditCard.Id);

                if (sourceStatement is null)
                {
                    continue;
                }

                var clonedStatement = new CreditCardStatement
                {
                    CreditCardId = creditCard.Id,
                    CompetenceDate = targetCompetenceDate,
                    ClosingDate = BuildDate(targetCompetenceDate.Year, targetCompetenceDate.Month, creditCard.ClosingDay),
                    DueDate = BuildDate(targetCompetenceDate.Year, targetCompetenceDate.Month, creditCard.DueDay),
                    TotalAmount = sourceStatement.TotalAmount,
                    Status = Domain.Enums.CreditCardStatementStatus.Open
                };

                await _creditCardStatementsWriteOnlyRepository.Add(clonedStatement);
                clonedStatement.CreditCard = creditCard;
                createdStatements.Add(clonedStatement);
            }

            if (createdObligations.Count > 0 || createdStatements.Count > 0)
            {
                await _unitOfWork.Commit();
            }

            return new ResponsePreparedMonth
            {
                CompetenceDate = targetCompetenceDate,
                SourceCompetenceDate = sourceCompetenceDate,
                IsMonthClosed = existingClosure is not null,
                IncomeSources = household.IncomeSources
                    .OrderBy(source => source.Name)
                    .Select(source => new ResponsePreparedIncomeSource
                    {
                        Id = source.Id,
                        Name = source.Name,
                        Amount = source.Amount,
                        IsRecurring = source.IsRecurring,
                        Frequency = (HouseholdIncomeFrequency)source.Frequency,
                        RequiresReview = household.HasVariableIncome
                            || !source.IsRecurring
                            || source.Frequency == Domain.Enums.HouseholdIncomeFrequency.Variable
                    })
                    .ToList(),
                CreatedObligations = createdObligations
                    .Select(obligation => _mapper.Map<ResponseFinancialObligation>(obligation))
                    .ToList(),
                CreatedCreditCardStatements = createdStatements
                    .Select(CreditCardStatementResponseMapper.Map)
                    .ToList(),
                SkippedObligations = skippedObligations,
                SkippedCreditCardStatements = skippedStatements
            };
        }

        private static bool IsRecurring(FinancialObligation obligation)
        {
            return obligation.RecurrenceType != Domain.Enums.FinancialObligationRecurrenceType.OneTime;
        }

        private static bool IsSameRecurringSeed(FinancialObligation existing, FinancialObligation source)
        {
            return existing.RecurrenceType == source.RecurrenceType
                && existing.Title.Equals(source.Title, StringComparison.OrdinalIgnoreCase)
                && existing.CategoryName.Equals(source.CategoryName, StringComparison.OrdinalIgnoreCase);
        }

        private static DateTime NormalizeCompetenceDate(DateTime competenceDate)
        {
            return new DateTime(competenceDate.Year, competenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        private static DateTime CopyDayToMonth(DateTime sourceDate, DateTime targetCompetenceDate)
        {
            return BuildDate(targetCompetenceDate.Year, targetCompetenceDate.Month, sourceDate.Day);
        }

        private static DateTime BuildDate(int year, int month, int day)
        {
            var maxDay = DateTime.DaysInMonth(year, month);
            return new DateTime(year, month, Math.Min(day, maxDay), 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
