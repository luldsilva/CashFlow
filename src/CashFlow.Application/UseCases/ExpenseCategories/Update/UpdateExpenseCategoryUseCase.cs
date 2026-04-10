using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.ExpenseCategories.Update
{
    public class UpdateExpenseCategoryUseCase : IUpdateExpenseCategoryUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateExpenseCategoryUseCase(
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseExpenseCategory> Execute(long id, RequestManageExpenseCategory request)
        {
            ExpenseCategoryRequestValidator.Validate(request);

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

            var categoryName = request.Name.Trim();
            if (household.ExpenseCategories.Any(item =>
                    item.Id != id && item.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException("Expense category already exists for the current user.");
            }

            category.Name = categoryName;
            category.BucketCode = ResolveBucketCode(household, request.BucketCode);

            await _unitOfWork.Commit();

            return ExpenseCategoryMapper.Map(category, household);
        }

        private static string? ResolveBucketCode(Domain.Entities.Household household, string? bucketCode)
        {
            if (string.IsNullOrWhiteSpace(bucketCode))
            {
                return null;
            }

            var resolvedBucket = household.PlanningBuckets
                .FirstOrDefault(bucket => bucket.Code.Equals(bucketCode.Trim(), StringComparison.OrdinalIgnoreCase));

            if (resolvedBucket is null)
            {
                throw new NotFoundException("Planning bucket was not found for the current user.");
            }

            return resolvedBucket.Code;
        }
    }
}
