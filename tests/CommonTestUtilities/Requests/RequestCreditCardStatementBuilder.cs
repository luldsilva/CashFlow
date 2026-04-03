using Bogus;
using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;

namespace CommonTestUtilities.Requests
{
    public class RequestCreditCardStatementBuilder
    {
        public static RequestCreditCardStatement Build(long creditCardId)
        {
            return new Faker<RequestCreditCardStatement>()
                .RuleFor(request => request.CreditCardId, _ => creditCardId)
                .RuleFor(request => request.CompetenceDate, _ => new DateTime(2026, 4, 1))
                .RuleFor(request => request.ClosingDate, _ => new DateTime(2026, 4, 25))
                .RuleFor(request => request.DueDate, _ => new DateTime(2026, 5, 5))
                .RuleFor(request => request.TotalAmount, faker => faker.Random.Decimal(500, 5000))
                .RuleFor(request => request.PaidAmount, _ => null)
                .RuleFor(request => request.PaidDate, _ => null)
                .RuleFor(request => request.Status, _ => CreditCardStatementStatus.Closed);
        }
    }
}
