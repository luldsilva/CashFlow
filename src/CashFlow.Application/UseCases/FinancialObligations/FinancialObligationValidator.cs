using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.FinancialObligations
{
    public class FinancialObligationValidator : AbstractValidator<RequestFinancialObligation>
    {
        public FinancialObligationValidator()
        {
            RuleFor(request => request.Title)
                .NotEmpty()
                .WithMessage("Financial obligation title is required.");

            RuleFor(request => request.CategoryName)
                .NotEmpty()
                .WithMessage("Financial obligation category is required.");

            RuleFor(request => request.CategoryId)
                .GreaterThan(0)
                .When(request => request.CategoryId.HasValue)
                .WithMessage("Financial obligation category id must be greater than zero.");

            RuleFor(request => request.Amount)
                .GreaterThan(0)
                .WithMessage("Financial obligation amount must be greater than zero.");

            RuleFor(request => request.CompetenceDate)
                .NotEmpty()
                .WithMessage("Financial obligation competence date is required.");

            RuleFor(request => request.DueDate)
                .NotEmpty()
                .WithMessage("Financial obligation due date is required.");

            RuleFor(request => request)
                .Must(HavePaidInformationWhenPaid)
                .WithMessage("Paid obligations must include paid amount and paid date.");

            RuleFor(request => request)
                .Must(NotHavePaidInformationWhenOpen)
                .WithMessage("Only paid obligations can include paid amount and paid date.");
        }

        private static bool HavePaidInformationWhenPaid(RequestFinancialObligation request)
        {
            if (request.Status != FinancialObligationStatus.Paid)
            {
                return true;
            }

            return request.PaidAmount.HasValue
                && request.PaidAmount.Value > 0
                && request.PaidDate.HasValue;
        }

        private static bool NotHavePaidInformationWhenOpen(RequestFinancialObligation request)
        {
            if (request.Status == FinancialObligationStatus.Paid)
            {
                return true;
            }

            return !request.PaidAmount.HasValue && !request.PaidDate.HasValue;
        }
    }
}
