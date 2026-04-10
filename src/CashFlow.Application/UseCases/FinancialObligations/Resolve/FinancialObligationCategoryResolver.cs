using CashFlow.Application.UseCases.ExpenseCategories.Resolve;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Entities;

namespace CashFlow.Application.UseCases.FinancialObligations.Resolve
{
    internal static class FinancialObligationCategoryResolver
    {
        public static ExpenseCategory? ResolveOptional(Household household, RequestFinancialObligation request)
        {
            return ExpenseCategoryResolver.ResolveOptional(household, request.CategoryId, request.CategoryName);
        }
    }
}
