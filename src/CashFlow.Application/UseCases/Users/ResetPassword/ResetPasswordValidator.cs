using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.Users.ResetPassword
{
    public class ResetPasswordValidator : AbstractValidator<RequestResetPassword>
    {
        public ResetPasswordValidator()
        {
            RuleFor(request => request.Token)
                .NotEmpty()
                .WithMessage("Reset token is required.");

            RuleFor(request => request.NewPassword)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("New password must contain at least 6 characters.");

            RuleFor(request => request.ConfirmNewPassword)
                .Equal(request => request.NewPassword)
                .WithMessage("New password confirmation does not match.");
        }
    }
}
