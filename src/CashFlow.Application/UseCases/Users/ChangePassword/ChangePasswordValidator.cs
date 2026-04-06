using CashFlow.Communication.Requests;
using FluentValidation;

namespace CashFlow.Application.UseCases.Users.ChangePassword
{
    public class ChangePasswordValidator : AbstractValidator<RequestChangePassword>
    {
        public ChangePasswordValidator()
        {
            RuleFor(request => request.CurrentPassword)
                .NotEmpty()
                .WithMessage("Current password is required.");

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
