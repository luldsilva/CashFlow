using CashFlow.Application.UseCases.Users.Register;
using CashFlow.Application.UseCases.Users.Shared;
using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using FluentValidation;

namespace Validators.Tests.Users
{
    public class PasswordValidatorTest
    {
        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        [InlineData(null)]
        [InlineData("aaaaaaaa")]
        [InlineData("AAAAAAAA")]
        [InlineData("Aaaaaaaa")]
        [InlineData("Aaaaaaa1")]
        public void Error_Name_Empty(string password)
        {
            var validator = new PasswordValidator<RequestRegisterUser>();

            var result = validator.IsValid(new ValidationContext<RequestRegisterUser>(new RequestRegisterUser()), password);

            result.Should().BeFalse();
        }
    }
}
