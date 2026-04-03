using AutoMapper;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialObligations.GetAll
{
    public class GetAllFinancialObligationsUseCase : IGetAllFinancialObligationsUseCase
    {
        private readonly IMapper _mapper;
        private readonly IFinancialObligationsReadOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;

        public GetAllFinancialObligationsUseCase(
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

        public async Task<ResponseFinancialObligations> Execute(DateTime competenceDate)
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing monthly obligations.");
            }

            var obligations = await _repository.GetAllByCompetence(household.Id, NormalizeCompetenceDate(competenceDate));

            return new ResponseFinancialObligations
            {
                Obligations = _mapper.Map<List<ResponseFinancialObligation>>(obligations)
            };
        }

        private static DateTime NormalizeCompetenceDate(DateTime competenceDate)
        {
            return new DateTime(competenceDate.Year, competenceDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
