using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.FinancialSetup
{
    public class FinancialSetupRequestValidator : AbstractValidator<RequestUpsertFinancialSetup>
    {
        public FinancialSetupRequestValidator()
        {
            RuleFor(request => request.HouseholdName)
                .NotEmpty()
                .WithMessage("Household name is required.");

            RuleFor(request => request.MembersCount)
                .GreaterThan(0)
                .WithMessage("Members count must be greater than zero.");

            RuleFor(request => request.IncomeSources)
                .NotEmpty()
                .WithMessage("At least one income source is required.");

            RuleForEach(request => request.IncomeSources)
                .SetValidator(new IncomeSourceValidator());

            RuleForEach(request => request.PlanningBuckets)
                .SetValidator(new PlanningBucketValidator());

            RuleForEach(request => request.ExpenseCategories)
                .SetValidator(new ExpenseCategoryValidator());

            RuleFor(request => request.PlanningBuckets)
                .Must(HaveUniqueBucketNames)
                .WithMessage("Planning buckets must have unique names.");

            RuleFor(request => request.ExpenseCategories)
                .Must(HaveUniqueCategoryNames)
                .WithMessage("Expense categories must have unique names.");

            RuleFor(request => request)
                .Must(HaveValidBucketAllocation)
                .WithMessage("Active planning buckets must sum 100%.");

            RuleFor(request => request)
                .Must(HaveCategoriesMappedToExistingBuckets)
                .WithMessage("Expense categories must reference an existing planning bucket.");
        }

        private static bool HaveUniqueBucketNames(List<RequestPlanningBucket> buckets)
        {
            return buckets
                .Select(bucket => bucket.Name.Trim().ToLowerInvariant())
                .Distinct()
                .Count() == buckets.Count;
        }

        private static bool HaveUniqueCategoryNames(List<RequestExpenseCategory> categories)
        {
            return categories
                .Select(category => category.Name.Trim().ToLowerInvariant())
                .Distinct()
                .Count() == categories.Count;
        }

        private static bool HaveValidBucketAllocation(RequestUpsertFinancialSetup request)
        {
            if (request.PlanningBuckets.Count == 0)
            {
                return true;
            }

            var activeBucketTotal = request.PlanningBuckets
                .Where(bucket => bucket.IsActive)
                .Sum(bucket => bucket.Percentage);

            return activeBucketTotal == 100m;
        }

        private static bool HaveCategoriesMappedToExistingBuckets(RequestUpsertFinancialSetup request)
        {
            if (request.ExpenseCategories.Count == 0)
            {
                return true;
            }

            var bucketNames = request.PlanningBuckets
                .Select(bucket => bucket.Name.Trim().ToLowerInvariant())
                .ToHashSet();

            return request.ExpenseCategories
                .Where(category => !string.IsNullOrWhiteSpace(category.BucketName))
                .All(category => bucketNames.Contains(category.BucketName!.Trim().ToLowerInvariant()));
        }

        private sealed class IncomeSourceValidator : AbstractValidator<RequestIncomeSource>
        {
            public IncomeSourceValidator()
            {
                RuleFor(source => source.Name)
                    .NotEmpty()
                    .WithMessage("Income source name is required.");

                RuleFor(source => source.Amount)
                    .GreaterThan(0)
                    .WithMessage("Income source amount must be greater than zero.");

                RuleFor(source => source.ExpectedDayOfMonth)
                    .InclusiveBetween((byte)1, (byte)31)
                    .When(source => source.ExpectedDayOfMonth.HasValue)
                    .WithMessage("Expected day of month must be between 1 and 31.");
            }
        }

        private sealed class PlanningBucketValidator : AbstractValidator<RequestPlanningBucket>
        {
            public PlanningBucketValidator()
            {
                RuleFor(bucket => bucket.Name)
                    .NotEmpty()
                    .WithMessage("Planning bucket name is required.");

                RuleFor(bucket => bucket.Percentage)
                    .GreaterThan(0)
                    .WithMessage("Planning bucket percentage must be greater than zero.");

                RuleFor(bucket => bucket.DisplayOrder)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Planning bucket display order must be zero or greater.");
            }
        }

        private sealed class ExpenseCategoryValidator : AbstractValidator<RequestExpenseCategory>
        {
            public ExpenseCategoryValidator()
            {
                RuleFor(category => category.Name)
                    .NotEmpty()
                    .WithMessage("Expense category name is required.");
            }
        }
    }

    public static class FinancialSetupValidator
    {
        public static void Validate(RequestUpsertFinancialSetup request)
        {
            var validator = new FinancialSetupRequestValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new CashFlow.Exception.ExceptionsBase.ErrorOnValidationException(
                    result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }
    }
}
