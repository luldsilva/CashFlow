using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.CreditCards;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.CreditCards.GetAll
{
    public class GetAllCreditCardsUseCase : IGetAllCreditCardsUseCase
    {
        private readonly ICreditCardsReadOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;

        public GetAllCreditCardsUseCase(
            ICreditCardsReadOnlyRepository repository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser)
        {
            _repository = repository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseCreditCards> Execute()
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing credit cards.");
            }

            var cards = await _repository.GetAll(household.Id);

            return new ResponseCreditCards
            {
                CreditCards = cards.Select(card => new ResponseCreditCard
                {
                    Id = card.Id,
                    Name = card.Name,
                    Brand = card.Brand,
                    LastFourDigits = card.LastFourDigits,
                    ClosingDay = card.ClosingDay,
                    DueDay = card.DueDay
                }).ToList()
            };
        }
    }
}
