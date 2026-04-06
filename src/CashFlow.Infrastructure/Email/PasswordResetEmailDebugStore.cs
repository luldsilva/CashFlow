namespace CashFlow.Infrastructure.Email
{
    public class PasswordResetEmailDebugStore
    {
        private readonly Dictionary<string, string> _latestLinksByEmail = [];

        public void Save(string email, string resetLink)
        {
            _latestLinksByEmail[email] = resetLink;
        }

        public string? GetLatestLink(string email)
        {
            return _latestLinksByEmail.TryGetValue(email, out var link) ? link : null;
        }
    }
}
