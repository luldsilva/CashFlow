using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialObligations.Delete
{
    public class DeleteFinancialObligationUseCase : IDeleteFinancialObligationUseCase
    {
        private readonly IFinancialObligationsReadOnlyRepository _readOnlyRepository;
        private readonly IFinancialObligationsWriteOnlyRepository _writeOnlyRepository;
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteFinancialObligationUseCase(
            IFinancialObligationsReadOnlyRepository readOnlyRepository,
            IFinancialObligationsWriteOnlyRepository writeOnlyRepository,
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(long id)
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing monthly obligations.");
            }

            var obligation = await _readOnlyRepository.GetById(household.Id, id);

            if (obligation is null)
            {
                throw new NotFoundException("Financial obligation was not found.");
            }

            await _writeOnlyRepository.Delete(id);
            await _unitOfWork.Commit();
        }
    }
}
