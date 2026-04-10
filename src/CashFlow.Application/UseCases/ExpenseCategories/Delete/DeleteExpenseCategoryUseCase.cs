using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Expenses;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.ExpenseCategories.Delete
{
    public class DeleteExpenseCategoryUseCase : IDeleteExpenseCategoryUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly IExpensesReadOnlyRepository _expensesRepository;
        private readonly IFinancialObligationsReadOnlyRepository _financialObligationsRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteExpenseCategoryUseCase(
            IHouseholdReadOnlyRepository householdRepository,
            IExpensesReadOnlyRepository expensesRepository,
            IFinancialObligationsReadOnlyRepository financialObligationsRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _householdRepository = householdRepository;
            _expensesRepository = expensesRepository;
            _financialObligationsRepository = financialObligationsRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(long id)
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing expense categories.");
            }

            var category = household.ExpenseCategories.FirstOrDefault(item => item.Id == id);

            if (category is null)
            {
                throw new NotFoundException("Expense category was not found.");
            }

            var isBeingUsed = await _financialObligationsRepository.ExistsByCategory(household.Id, id)
                || await _expensesRepository.ExistsByCategory(loggedUser.Id, id);

            if (isBeingUsed)
            {
                throw new ConflictException("Expense category is being used by existing records.");
            }

            household.ExpenseCategories.Remove(category);
            await _unitOfWork.Commit();
        }
    }
}
