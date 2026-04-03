using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialObligations.GetById
{
    public class GetFinancialObligationByIdUseCase : IGetFinancialObligationByIdUseCase
    {
        private readonly IMapper _mapper;
        private readonly IFinancialObligationsReadOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;

        public GetFinancialObligationByIdUseCase(
            IMapper mapper,
            IFinancialObligationsReadOnlyRepository repository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser)
        {
            _mapper = mapper;
            _repository = repository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseFinancialObligation> Execute(long id)
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing monthly obligations.");
            }

            var obligation = await _repository.GetById(household.Id, id);

            if (obligation is null)
            {
                throw new NotFoundException("Financial obligation was not found.");
            }

            return _mapper.Map<ResponseFinancialObligation>(obligation);
        }
    }
}
