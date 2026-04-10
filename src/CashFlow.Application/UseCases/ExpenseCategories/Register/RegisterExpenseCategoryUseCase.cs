using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.ExpenseCategories.Register
{
    public class RegisterExpenseCategoryUseCase : IRegisterExpenseCategoryUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterExpenseCategoryUseCase(
            IHouseholdReadOnlyRepository householdRepository,
            ILoggedUser loggedUser,
            IUnitOfWork unitOfWork)
        {
            _householdRepository = householdRepository;
            _loggedUser = loggedUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseExpenseCategory> Execute(RequestManageExpenseCategory request)
        {
            ExpenseCategoryRequestValidator.Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                throw new NotFoundException("Financial setup must be created before managing expense categories.");
            }

            var categoryName = request.Name.Trim();
            if (household.ExpenseCategories.Any(category => category.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ConflictException("Expense category already exists for the current user.");
            }

            var bucketCode = ResolveBucketCode(household, request.BucketCode);

            var category = new ExpenseCategory
            {
                Name = categoryName,
                BucketCode = bucketCode
            };

            household.ExpenseCategories.Add(category);
            await _unitOfWork.Commit();

            return ExpenseCategoryMapper.Map(category, household);
        }

        private static string? ResolveBucketCode(Household household, string? bucketCode)
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
