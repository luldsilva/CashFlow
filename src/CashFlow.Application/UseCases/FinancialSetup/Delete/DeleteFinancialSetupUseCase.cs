using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialSetup.Delete
{
    public class DeleteFinancialSetupUseCase : IDeleteFinancialSetupUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdReadOnlyRepository;
        private readonly IHouseholdWriteOnlyRepository _householdWriteOnlyRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFinancialSetupUseCase(
            IHouseholdReadOnlyRepository householdReadOnlyRepository,
            IHouseholdWriteOnlyRepository householdWriteOnlyRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _householdReadOnlyRepository = householdReadOnlyRepository;
            _householdWriteOnlyRepository = householdWriteOnlyRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute()
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdReadOnlyRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup was not found for the current user.");
            }

            await _householdWriteOnlyRepository.Delete(household.Id);
            await _unitOfWork.Commit();
        }
    }
}
