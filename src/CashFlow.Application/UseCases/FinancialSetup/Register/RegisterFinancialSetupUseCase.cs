using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Application.UseCases.FinancialSetup;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialSetup.Register
{
    public class RegisterFinancialSetupUseCase : IRegisterFinancialSetupUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdReadOnlyRepository;
        private readonly IHouseholdWriteOnlyRepository _householdWriteOnlyRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterFinancialSetupUseCase(
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

        public async Task<ResponseFinancialSetup> Execute(RequestUpsertFinancialSetup request)
        {
            FinancialSetupValidator.Validate(request);

            var loggedUser = await _loggedUser.Get();
            var existingHousehold = await _householdReadOnlyRepository.GetByUserId(loggedUser.Id);

            if (existingHousehold is not null)
            {
                throw new ConflictException("Financial setup already exists for the current user.");
            }

            var household = FinancialSetupMapper.MapToNewHousehold(request, loggedUser.Id);

            await _householdWriteOnlyRepository.Add(household);
            await _unitOfWork.Commit();

            return FinancialSetupMapper.MapToResponse(household);
        }
    }
}
