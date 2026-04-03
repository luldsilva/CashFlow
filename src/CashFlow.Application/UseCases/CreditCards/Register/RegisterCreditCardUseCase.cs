using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.CreditCards;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.CreditCards.Register
{
    public class RegisterCreditCardUseCase : IRegisterCreditCardUseCase
    {
        private readonly ICreditCardsWriteOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCreditCardUseCase(
            ICreditCardsWriteOnlyRepository repository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseCreditCard> Execute(RequestCreditCard request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing credit cards.");
            }

            var creditCard = new CreditCard
            {
                HouseholdId = household.Id,
                Name = request.Name.Trim(),
                Brand = request.Brand.Trim(),
                LastFourDigits = request.LastFourDigits.Trim(),
                ClosingDay = request.ClosingDay,
                DueDay = request.DueDay
            };

            await _repository.Add(creditCard);
            await _unitOfWork.Commit();

            return new ResponseCreditCard
            {
                Id = creditCard.Id,
                Name = creditCard.Name,
                Brand = creditCard.Brand,
                LastFourDigits = creditCard.LastFourDigits,
                ClosingDay = creditCard.ClosingDay,
                DueDay = creditCard.DueDay
            };
        }

        private static void Validate(RequestCreditCard request)
        {
            var validator = new CreditCardValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }
    }
}
