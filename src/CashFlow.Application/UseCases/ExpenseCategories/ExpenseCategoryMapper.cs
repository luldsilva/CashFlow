using CashFlow.Communication.Responses;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.UseCases.ExpenseCategories
{
    internal static class ExpenseCategoryMapper
    {
        public static ResponseExpenseCategory Map(ExpenseCategory category, Household household)
        {
            var bucket = string.IsNullOrWhiteSpace(category.BucketCode)
                ? null
                : household.PlanningBuckets.FirstOrDefault(bucket =>
                    bucket.Code.Equals(category.BucketCode, StringComparison.OrdinalIgnoreCase));

            return new ResponseExpenseCategory
            {
                Id = category.Id,
                Name = category.Name,
                BucketCode = category.BucketCode,
                BucketName = bucket?.Name
            };
        }
    }
}
