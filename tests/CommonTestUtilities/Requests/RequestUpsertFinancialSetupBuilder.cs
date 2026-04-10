using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestUpsertFinancialSetupBuilder
    {
        public static RequestUpsertFinancialSetup Build()
        {
            return new RequestUpsertFinancialSetup
            {
                HouseholdName = "Familia Silva",
                MembersCount = 3,
                HasVariableIncome = true,
                PrimaryIncomeFrequency = HouseholdIncomeFrequency.Monthly,
                PlanningModel = PlanningModel.Balanced,
                IncomeSources =
                [
                    new RequestIncomeSource
                    {
                        Name = "Pro labore",
                        Type = IncomeSourceType.Business,
                        Amount = 12000m,
                        IsRecurring = true,
                        Frequency = HouseholdIncomeFrequency.Monthly,
                        ExpectedDayOfMonth = 5
                    },
                    new RequestIncomeSource
                    {
                        Name = "Freela",
                        Type = IncomeSourceType.Freelance,
                        Amount = 1500m,
                        IsRecurring = false,
                        Frequency = HouseholdIncomeFrequency.Variable
                    }
                ],
                PlanningBuckets =
                [
                    new RequestPlanningBucket
                    {
                        Name = "Essenciais",
                        Percentage = 50m,
                        IsActive = true,
                        DisplayOrder = 0
                    },
                    new RequestPlanningBucket
                    {
                        Name = "Investimentos",
                        Percentage = 20m,
                        IsActive = true,
                        DisplayOrder = 1
                    },
                    new RequestPlanningBucket
                    {
                        Name = "Livre",
                        Percentage = 30m,
                        IsActive = true,
                        DisplayOrder = 2
                    }
                ],
                ExpenseCategories =
                [
                    new RequestExpenseCategory
                    {
                        Name = "Aluguel",
                        BucketName = "Essenciais"
                    },
                    new RequestExpenseCategory
                    {
                        Name = "Mercado",
                        BucketName = "Essenciais"
                    },
                    new RequestExpenseCategory
                    {
                        Name = "Corretora",
                        BucketName = "Investimentos"
                    }
                ]
            };
        }
    }
}
