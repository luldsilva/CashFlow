namespace CashFlow.Infrastructure.PasswordReset
{
    public class PasswordResetSettings
    {
        public string BaseUrl { get; set; } = "http://localhost:3000/reset-password";
        public int TokenExpiresMinutes { get; set; } = 30;
    }
}
