using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.CreditCardStatements
{
    public class CreditCardStatementValidator : AbstractValidator<RequestCreditCardStatement>
    {
        public CreditCardStatementValidator()
        {
            RuleFor(request => request.CreditCardId).GreaterThan(0).WithMessage("Credit card id is required.");
            RuleFor(request => request.TotalAmount).GreaterThan(0).WithMessage("Credit card statement total amount must be greater than zero.");
            RuleFor(request => request)
                .Must(HavePaidInformationWhenPaid)
                .WithMessage("Paid credit card statements must include paid amount and paid date.");
        }

        private static bool HavePaidInformationWhenPaid(RequestCreditCardStatement request)
        {
            if (request.Status != CreditCardStatementStatus.Paid)
            {
                return true;
            }

            return request.PaidAmount.HasValue
                && request.PaidAmount.Value > 0
                && request.PaidDate.HasValue;
        }
    }
}
