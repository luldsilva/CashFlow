namespace CashFlow.Domain.Services.Email
{
    public interface IEmailSender
    {
        Task Send(string to, string subject, string htmlBody);
    }
}
