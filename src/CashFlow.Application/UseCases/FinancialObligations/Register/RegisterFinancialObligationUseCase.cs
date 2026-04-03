using AutoMapper;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialObligations.Register
{
    public class RegisterFinancialObligationUseCase : IRegisterFinancialObligationUseCase
    {
        private readonly IMapper _mapper;
        private readonly IFinancialObligationsWriteOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterFinancialObligationUseCase(
            IMapper mapper,
            IFinancialObligationsWriteOnlyRepository repository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _repository = repository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseFinancialObligation> Execute(RequestFinancialObligation request)
        {
            Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing monthly obligations.");
            }

            var obligation = _mapper.Map<FinancialObligation>(request);
            obligation.HouseholdId = household.Id;
            obligation.CompetenceDate = NormalizeCompetenceDate(request.CompetenceDate);

            await _repository.Add(obligation);
            await _unitOfWork.Commit();

            return _mapper.Map<ResponseFinancialObligation>(obligation);
        }

        private static void Validate(RequestFinancialObligation request)
        {
            var validator = new FinancialObligationValidator();
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
