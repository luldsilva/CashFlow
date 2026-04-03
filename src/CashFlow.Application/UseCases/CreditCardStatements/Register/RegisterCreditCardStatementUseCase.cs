using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.CreditCards;
using CashFlow.Domain.Repositories.CreditCardStatements;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.CreditCardStatements.Register
{
    public class RegisterCreditCardStatementUseCase : IRegisterCreditCardStatementUseCase
    {
        private readonly ICreditCardStatementsWriteOnlyRepository _repository;
        private readonly ICreditCardsReadOnlyRepository _creditCardsRepository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCreditCardStatementUseCase(
            ICreditCardStatementsWriteOnlyRepository repository,
            ICreditCardsReadOnlyRepository creditCardsRepository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _creditCardsRepository = creditCardsRepository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseCreditCardStatement> Execute(RequestCreditCardStatement request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing credit cards.");
            }

            var creditCard = await _creditCardsRepository.GetById(household.Id, request.CreditCardId);

            if (creditCard is null)
            {
                throw new NotFoundException("Credit card was not found.");
            }

            var statement = new CreditCardStatement
            {
                CreditCardId = creditCard.Id,
                CompetenceDate = NormalizeCompetenceDate(request.CompetenceDate),
                ClosingDate = request.ClosingDate,
                DueDate = request.DueDate,
                TotalAmount = request.TotalAmount,
                PaidAmount = request.PaidAmount,
                PaidDate = request.PaidDate,
                Status = (Domain.Enums.CreditCardStatementStatus)request.Status
            };

            await _repository.Add(statement);
            await _unitOfWork.Commit();

            return new ResponseCreditCardStatement
            {
                Id = statement.Id,
                CreditCardId = creditCard.Id,
                CreditCardName = creditCard.Name,
                CompetenceDate = statement.CompetenceDate,
                ClosingDate = statement.ClosingDate,
                DueDate = statement.DueDate,
                TotalAmount = statement.TotalAmount,
                PaidAmount = statement.PaidAmount,
                PaidDate = statement.PaidDate,
                Status = request.Status
            };
        }

        private static void Validate(RequestCreditCardStatement request)
        {
            var validator = new CreditCardStatementValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }

        private static DateTime NormalizeCompetenceDate(DateTime competenceDate)
        {
            return new DateTime(competenceDate.Year, competenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
