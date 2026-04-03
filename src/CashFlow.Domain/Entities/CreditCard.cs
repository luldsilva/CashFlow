namespace CashFlow.Domain.Entities
{
    public class CreditCard
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;
        public byte ClosingDay { get; set; }
        public byte DueDay { get; set; }

        public long HouseholdId { get; set; }
        public Household Household { get; set; } = default!;
        public List<CreditCardStatement> Statements { get; set; } = [];
    }
}
