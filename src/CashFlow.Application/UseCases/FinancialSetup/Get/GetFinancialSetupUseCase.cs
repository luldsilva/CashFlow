using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialSetup.Get
{
    public class GetFinancialSetupUseCase : IGetFinancialSetupUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdReadOnlyRepository;
        private readonly ILoggedUser _loggedUser;

        public GetFinancialSetupUseCase(IHouseholdReadOnlyRepository householdReadOnlyRepository, ILoggedUser loggedUser)
        {
            _householdReadOnlyRepository = householdReadOnlyRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseFinancialSetup> Execute()
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdReadOnlyRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup was not found for the current user.");
            }

            return FinancialSetupMapper.MapToResponse(household);
        }
    }
}
