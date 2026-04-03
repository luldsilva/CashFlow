using Bogus;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestCreditCardBuilder
    {
        public static RequestCreditCard Build()
        {
            return new Faker<RequestCreditCard>()
                .RuleFor(request => request.Name, faker => $"Cartao {faker.Company.CompanyName()}")
                .RuleFor(request => request.Brand, faker => faker.PickRandom("Visa", "Mastercard"))
                .RuleFor(request => request.LastFourDigits, faker => faker.Random.ReplaceNumbers("####"))
                .RuleFor(request => request.ClosingDay, faker => (byte)faker.Random.Int(1, 28))
                .RuleFor(request => request.DueDay, faker => (byte)faker.Random.Int(1, 28));
        }
    }
}
