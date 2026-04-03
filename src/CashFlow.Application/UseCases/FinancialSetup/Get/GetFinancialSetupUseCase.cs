using CashFlow.Communication.Enums;
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

            return new ResponseFinancialSetup
            {
                Id = household.Id,
                HouseholdName = household.Name,
                MembersCount = household.MembersCount,
                HasVariableIncome = household.HasVariableIncome,
                PrimaryIncomeFrequency = (HouseholdIncomeFrequency)household.PrimaryIncomeFrequency,
                PlanningModel = (PlanningModel)household.PlanningModel,
                IncomeSources = household.IncomeSources
                    .OrderBy(source => source.Name)
                    .Select(source => new ResponseIncomeSource
                    {
                        Id = source.Id,
                        Name = source.Name,
                        Type = (IncomeSourceType)source.Type,
                        Amount = source.Amount,
                        IsRecurring = source.IsRecurring,
                        Frequency = (HouseholdIncomeFrequency)source.Frequency,
                        ExpectedDayOfMonth = source.ExpectedDayOfMonth
                    })
                    .ToList(),
                PlanningBuckets = household.PlanningBuckets
                    .OrderBy(bucket => bucket.DisplayOrder)
                    .Select(bucket => new ResponsePlanningBucket
                    {
                        Id = bucket.Id,
                        Code = bucket.Code,
                        Name = bucket.Name,
                        Percentage = bucket.Percentage,
                        IsActive = bucket.IsActive,
                        DisplayOrder = bucket.DisplayOrder
                    })
                    .ToList(),
                ExpenseCategories = household.ExpenseCategories
                    .OrderBy(category => category.Name)
                    .Select(category => new ResponseExpenseCategory
                    {
                        Id = category.Id,
                        Name = category.Name,
                        BucketCode = category.BucketCode
                    })
                    .ToList()
            };
        }
    }
}
