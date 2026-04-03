namespace CashFlow.Communication.Responses
{
    public class ResponseCreditCard
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;
        public byte ClosingDay { get; set; }
        public byte DueDay { get; set; }
    }
}
