using CashFlow.Domain.Services.Email;
using Microsoft.Extensions.Logging;

namespace CashFlow.Infrastructure.Email
{
    public class DevelopmentEmailSender : IEmailSender
    {
        private readonly ILogger<DevelopmentEmailSender> _logger;
        private readonly PasswordResetEmailDebugStore _debugStore;

        public DevelopmentEmailSender(ILogger<DevelopmentEmailSender> logger, PasswordResetEmailDebugStore debugStore)
        {
            _logger = logger;
            _debugStore = debugStore;
        }

        public Task Send(string to, string subject, string htmlBody)
        {
            var resetLink = ExtractHref(htmlBody);

            if (!string.IsNullOrWhiteSpace(resetLink))
            {
                _debugStore.Save(to, resetLink);
            }

            _logger.LogInformation("DEV EMAIL to {Email}. Subject: {Subject}. Body: {Body}", to, subject, htmlBody);

            return Task.CompletedTask;
        }

        private static string ExtractHref(string htmlBody)
        {
            const string hrefPrefix = "href=\"";
            var start = htmlBody.IndexOf(hrefPrefix, StringComparison.OrdinalIgnoreCase);

            if (start < 0)
            {
                return string.Empty;
            }

            start += hrefPrefix.Length;
            var end = htmlBody.IndexOf('"', start);

            return end < 0 ? string.Empty : htmlBody[start..end];
        }
    }
}
