using Bogus;
using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestFinancialObligationBuilder
    {
        public static RequestFinancialObligation Build()
        {
            return new Faker<RequestFinancialObligation>()
                .RuleFor(request => request.Title, faker => faker.Commerce.ProductName())
                .RuleFor(request => request.Description, faker => faker.Commerce.ProductDescription())
                .RuleFor(request => request.CategoryName, faker => faker.PickRandom("Aluguel", "Internet", "Energia"))
                .RuleFor(request => request.BucketCode, faker => faker.PickRandom("essentials", "free"))
                .RuleFor(request => request.Amount, faker => faker.Random.Decimal(50, 5000))
                .RuleFor(request => request.PaidAmount, _ => null)
                .RuleFor(request => request.CompetenceDate, _ => new DateTime(2026, 4, 1))
                .RuleFor(request => request.DueDate, faker => new DateTime(2026, 4, faker.Random.Int(1, 28)))
                .RuleFor(request => request.PaidDate, _ => null)
                .RuleFor(request => request.RecurrenceType, faker => faker.PickRandom(FinancialObligationRecurrenceType.FixedMonthly, FinancialObligationRecurrenceType.VariableMonthly))
                .RuleFor(request => request.Status, _ => FinancialObligationStatus.Expected);
        }
    }
}
