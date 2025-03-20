using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestRegisterUserBuilder
    {
        public static RequestRegisterUser Build()
        {
            return new Faker<RequestRegisterUser>()
                .RuleFor(user => user.Name, faker => faker.Person.FirstName)
                .RuleFor(user => user.Email, (faker, user) => faker.Internet.Email(user.Name))
                .RuleFor(user => user.Password, faker => faker.Internet.Password(prefix: "!Aa1"));
        }
    }
}
