using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.CreditCardStatements;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.CreditCardStatements.GetByMonth
{
    public class GetCreditCardStatementsByMonthUseCase : IGetCreditCardStatementsByMonthUseCase
    {
        private readonly ICreditCardStatementsReadOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;

        public GetCreditCardStatementsByMonthUseCase(
            ICreditCardStatementsReadOnlyRepository repository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser)
        {
            _repository = repository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseCreditCardStatements> Execute(DateTime competenceDate)
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing credit cards.");
            }

            var normalizedCompetenceDate = new DateTime(competenceDate.Year, competenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var statements = await _repository.GetByMonth(household.Id, normalizedCompetenceDate);

            return new ResponseCreditCardStatements
            {
                Statements = statements.Select(statement => new ResponseCreditCardStatement
                {
                    Id = statement.Id,
                    CreditCardId = statement.CreditCardId,
                    CreditCardName = statement.CreditCard.Name,
                    CompetenceDate = statement.CompetenceDate,
                    ClosingDate = statement.ClosingDate,
                    DueDate = statement.DueDate,
                    TotalAmount = statement.TotalAmount,
                    PaidAmount = statement.PaidAmount,
                    PaidDate = statement.PaidDate,
                    Status = (CashFlow.Communication.Enums.CreditCardStatementStatus)statement.Status
                }).ToList()
            };
        }
    }
}
