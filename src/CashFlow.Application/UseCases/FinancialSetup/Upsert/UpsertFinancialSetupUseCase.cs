using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.FinancialSetup.Upsert
{
    public class UpsertFinancialSetupUseCase : IUpsertFinancialSetupUseCase
    {
        private readonly IHouseholdReadOnlyRepository _householdReadOnlyRepository;
        private readonly IHouseholdWriteOnlyRepository _householdWriteOnlyRepository;
        private readonly ILoggedUser _loggedUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpsertFinancialSetupUseCase(
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
            Validate(request);

            var loggedUser = await _loggedUser.Get();
            var household = await _householdReadOnlyRepository.GetByUserId(loggedUser.Id);

            if (household is null)
            {
                household = MapToNewHousehold(request, loggedUser.Id);
                await _householdWriteOnlyRepository.Add(household);
            }
            else
            {
                ApplyChanges(household, request);
            }

            await _unitOfWork.Commit();

            return MapToResponse(household);
        }

        private static void Validate(RequestUpsertFinancialSetup request)
        {
            var validator = new UpsertFinancialSetupValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }

        private static Household MapToNewHousehold(RequestUpsertFinancialSetup request, long userId)
        {
            var household = new Household
            {
                UserId = userId
            };

            ApplyChanges(household, request);

            return household;
        }

        private static void ApplyChanges(Household household, RequestUpsertFinancialSetup request)
        {
            household.Name = request.HouseholdName.Trim();
            household.MembersCount = request.MembersCount;
            household.HasVariableIncome = request.HasVariableIncome;
            household.PrimaryIncomeFrequency = (Domain.Enums.HouseholdIncomeFrequency)request.PrimaryIncomeFrequency;
            household.PlanningModel = (Domain.Enums.PlanningModel)request.PlanningModel;

            household.IncomeSources.Clear();
            foreach (var source in request.IncomeSources)
            {
                household.IncomeSources.Add(new IncomeSource
                {
                    Name = source.Name.Trim(),
                    Type = (Domain.Enums.IncomeSourceType)source.Type,
                    Amount = source.Amount,
                    IsRecurring = source.IsRecurring,
                    Frequency = (Domain.Enums.HouseholdIncomeFrequency)source.Frequency,
                    ExpectedDayOfMonth = source.ExpectedDayOfMonth
                });
            }

            household.PlanningBuckets.Clear();
            foreach (var bucket in request.PlanningBuckets.OrderBy(bucket => bucket.DisplayOrder))
            {
                household.PlanningBuckets.Add(new PlanningBucket
                {
                    Code = bucket.Code.Trim(),
                    Name = bucket.Name.Trim(),
                    Percentage = bucket.Percentage,
                    IsActive = bucket.IsActive,
                    DisplayOrder = bucket.DisplayOrder
                });
            }

            household.ExpenseCategories.Clear();
            foreach (var category in request.ExpenseCategories.OrderBy(category => category.Name))
            {
                household.ExpenseCategories.Add(new ExpenseCategory
                {
                    Name = category.Name.Trim(),
                    BucketCode = category.BucketCode.Trim()
                });
            }
        }

        private static ResponseFinancialSetup MapToResponse(Household household)
        {
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
