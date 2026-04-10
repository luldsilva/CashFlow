using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.ExpenseCategories.GetAll
{
    public class GetAllExpenseCategoriesUseCase : IGetAllExpenseCategoriesUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;

        public GetAllExpenseCategoriesUseCase(
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser)
        {
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
        }

        public async Task<ResponseExpenseCategories> Execute()
        {
            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing expense categories.");
            }

            return new ResponseExpenseCategories
            {
                Categories = household.ExpenseCategories
                    .OrderBy(category => category.Name)
                    .Select(category => ExpenseCategoryMapper.Map(category, household))
                    .ToList()
            };
        }
    }
}
