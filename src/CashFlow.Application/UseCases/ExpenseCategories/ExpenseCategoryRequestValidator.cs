using CashFlow.Communication.Requests;
using CashFlow.Exception.ExceptionsBase;
using FluentValidation;

namespace CashFlow.Application.UseCases.ExpenseCategories
{
    internal static class ExpenseCategoryRequestValidator
    {
        public static void Validate(RequestManageExpenseCategory request)
        {
            var validator = new InlineValidator<RequestManageExpenseCategory>();

            validator.RuleFor(category => category.Name)
                .NotEmpty()
                .WithMessage("Expense category name is required.");

            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }
    }
}
