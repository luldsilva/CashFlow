using CashFlow.Domain.Entities;
using CashFlow.Exception.ExceptionsBase;

namespace CashFlow.Application.UseCases.ExpenseCategories.Resolve
{
    internal static class ExpenseCategoryResolver
    {
        public static ExpenseCategory? ResolveOptional(Household? household, long? categoryId, string? categoryName)
        {
            if (household is null)
            {
                return null;
            }

            if (categoryId.HasValue)
            {
                var categoryById = household.ExpenseCategories.FirstOrDefault(category => category.Id == categoryId.Value);
                if (categoryById is null)
                {
                    throw new NotFoundException("Expense category was not found for the current user.");
                }

                return categoryById;
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return null;
            }

            return household.ExpenseCategories
                .FirstOrDefault(category => category.Name.Equals(categoryName.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}
