using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestChangePasswordBuilder
    {
        public static RequestChangePassword Build(string currentPassword)
        {
            return new Faker<RequestChangePassword>()
                .RuleFor(request => request.CurrentPassword, _ => currentPassword)
                .RuleFor(request => request.NewPassword, faker => faker.Internet.Password(length: 8, memorable: true))
                .RuleFor(request => request.ConfirmNewPassword, (faker, request) => request.NewPassword);
        }
    }
}
