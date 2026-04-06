using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.Users.ForgotPassword
{
    public class ForgotPasswordValidator : AbstractValidator<RequestForgotPassword>
    {
        public ForgotPasswordValidator()
        {
            RuleFor(request => request.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("A valid email is required.");
        }
    }
}
