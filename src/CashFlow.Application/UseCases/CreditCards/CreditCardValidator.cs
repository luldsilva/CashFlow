using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.CreditCards
{
    public class CreditCardValidator : AbstractValidator<RequestCreditCard>
    {
        public CreditCardValidator()
        {
            RuleFor(request => request.Name).NotEmpty().WithMessage("Credit card name is required.");
            RuleFor(request => request.Brand).NotEmpty().WithMessage("Credit card brand is required.");
            RuleFor(request => request.LastFourDigits)
                .Length(4)
                .Matches("^[0-9]{4}$")
                .WithMessage("Credit card last four digits must contain exactly 4 numbers.");
            RuleFor(request => request.ClosingDay).InclusiveBetween((byte)1, (byte)31).WithMessage("Credit card closing day must be between 1 and 31.");
            RuleFor(request => request.DueDay).InclusiveBetween((byte)1, (byte)31).WithMessage("Credit card due day must be between 1 and 31.");
        }
    }
}
