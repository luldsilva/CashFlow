using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Application.UseCases.FinancialSetup;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialSetup.Update
{
    public class UpdateFinancialSetupUseCase : IUpdateFinancialSetupUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdReadOnlyRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateFinancialSetupUseCase(
            IHouseholdReadOnlyRepository householdReadOnlyRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _householdReadOnlyRepository = householdReadOnlyRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseFinancialSetup> Execute(RequestUpsertFinancialSetup request)
        {
            FinancialSetupValidator.Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdReadOnlyRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup was not found for the current user.");
            }

            FinancialSetupMapper.ApplyChanges(household, request);

            await _unitOfWork.Commit();

            return FinancialSetupMapper.MapToResponse(household);
        }
    }
}
