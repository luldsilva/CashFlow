using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CashFlow.Application.UseCases.FinancialSetup
{
    internal static class FinancialSetupMapper
    {
        public static Household MapToNewHousehold(RequestUpsertFinancialSetup request, long userId)
        {
            var household = new Household
            {
                UserId = userId,
                PlanningModel = Domain.Enums.PlanningModel.Balanced
            };

            ApplyChanges(household, request);

            return household;
        }

        public static void ApplyChanges(Household household, RequestUpsertFinancialSetup request)
        {
            var bucketCodesByName = GenerateBucketCodesByName(request.PlanningBuckets);
            var shouldReplacePlanningBuckets = request.PlanningBuckets.Count > 0 || household.Id == 0;
            var shouldReplaceExpenseCategories = request.ExpenseCategories.Count > 0 || household.Id == 0;

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

            if (shouldReplacePlanningBuckets)
            {
                household.PlanningBuckets.Clear();
                foreach (var bucket in request.PlanningBuckets.OrderBy(bucket => bucket.DisplayOrder))
                {
                    household.PlanningBuckets.Add(new PlanningBucket
                    {
                        Code = bucketCodesByName[NormalizeKey(bucket.Name)],
                        Name = bucket.Name.Trim(),
                        Percentage = bucket.Percentage,
                        IsActive = bucket.IsActive,
                        DisplayOrder = bucket.DisplayOrder
                    });
                }
            }

            if (shouldReplaceExpenseCategories)
            {
                household.ExpenseCategories.Clear();
                foreach (var category in request.ExpenseCategories.OrderBy(category => category.Name))
                {
                    household.ExpenseCategories.Add(new ExpenseCategory
                    {
                        Name = category.Name.Trim(),
                        BucketCode = string.IsNullOrWhiteSpace(category.BucketName)
                            ? null
                            : bucketCodesByName[NormalizeKey(category.BucketName)]
                    });
                }
            }
        }

        public static ResponseFinancialSetup MapToResponse(Household household)
        {
            return new ResponseFinancialSetup
            {
                Id = household.Id,
                HouseholdName = household.Name,
                MembersCount = household.MembersCount,
                HasVariableIncome = household.HasVariableIncome,
                PrimaryIncomeFrequency = (HouseholdIncomeFrequency)household.PrimaryIncomeFrequency,
                PlanningModel = (PlanningModel)household.PlanningModel,
                HasPlanningConfigured = household.PlanningBuckets.Any(bucket => bucket.IsActive),
                HasExpenseCategoriesConfigured = household.ExpenseCategories.Count > 0,
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
                        BucketName = string.IsNullOrWhiteSpace(category.BucketCode)
                            ? null
                            : household.PlanningBuckets
                                .FirstOrDefault(bucket => bucket.Code.Equals(category.BucketCode, StringComparison.OrdinalIgnoreCase))
                                ?.Name,
                        BucketCode = category.BucketCode
                    })
                    .ToList()
            };
        }

        private static Dictionary<string, string> GenerateBucketCodesByName(IEnumerable<RequestPlanningBucket> buckets)
        {
            var result = new Dictionary<string, string>();
            var usedCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var bucket in buckets.OrderBy(bucket => bucket.DisplayOrder))
            {
                var normalizedName = NormalizeKey(bucket.Name);
                var baseCode = GenerateCanonicalCode(bucket.Name);
                var code = baseCode;
                var suffix = 2;

                while (!usedCodes.Add(code))
                {
                    code = $"{baseCode}-{suffix}";
                    suffix++;
                }

                result[normalizedName] = code;
            }

            return result;
        }

        private static string GenerateCanonicalCode(string name)
        {
            var normalizedName = NormalizeKey(name);

            return normalizedName switch
            {
                "essenciais" => "essentials",
                "essencial" => "essentials",
                "essentials" => "essentials",
                "essential" => "essentials",
                "educacao" => "education",
                "education" => "education",
                "investimentos" => "investments",
                "investimento" => "investments",
                "investments" => "investments",
                "investment" => "investments",
                "aposentadoria" => "retirement",
                "retirement" => "retirement",
                "livre" => "free",
                "free" => "free",
                "lazer" => "leisure",
                "leisure" => "leisure",
                "reserva" => "reserve",
                "reserve" => "reserve",
                "doacao" => "donation",
                "donation" => "donation",
                _ => normalizedName
            };
        }

        private static string NormalizeKey(string value)
        {
            var trimmed = value.Trim();
            var normalized = trimmed.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(character);
                }
            }

            var withoutDiacritics = builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();
            var slug = Regex.Replace(withoutDiacritics, "[^a-z0-9]+", "-").Trim('-');

            return string.IsNullOrWhiteSpace(slug) ? "bucket" : slug;
        }
    }
}
