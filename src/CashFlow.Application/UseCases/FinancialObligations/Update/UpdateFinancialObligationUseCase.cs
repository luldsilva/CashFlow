using AutoMapper;
using CashFlow.Application.UseCases.FinancialObligations.Resolve;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialObligations.Update
{
    public class UpdateFinancialObligationUseCase : IUpdateFinancialObligationUseCase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFinancialObligationsUpdateOnlyRepository _repository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;

        public UpdateFinancialObligationUseCase(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IFinancialObligationsUpdateOnlyRepository repository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
        }

        public async Task Execute(long id, RequestFinancialObligation request)
        {
            Validate(request);

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

            _mapper.Map(request, obligation);
            obligation.CompetenceDate = NormalizeCompetenceDate(request.CompetenceDate);
            obligation.Title = obligation.Title.Trim();
            obligation.Description = string.IsNullOrWhiteSpace(obligation.Description) ? null : obligation.Description.Trim();
            obligation.BucketCode = request.BucketCode?.Trim() ?? string.Empty;

            var category = FinancialObligationCategoryResolver.ResolveOptional(household, request);
            obligation.ExpenseCategoryId = category?.Id;
            obligation.ExpenseCategory = category;
            obligation.CategoryName = category?.Name ?? obligation.CategoryName.Trim();

            _repository.Update(obligation);
            await _unitOfWork.Commit();
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
