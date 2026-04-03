namespace CashFlow.Communication.Requests
{
    public class RequestCreditCard
    {
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string LastFourDigits { get; set; } = string.Empty;
        public byte ClosingDay { get; set; }
        public byte DueDay { get; set; }
    }
}
