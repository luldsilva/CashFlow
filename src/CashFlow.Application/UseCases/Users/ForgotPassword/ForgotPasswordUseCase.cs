using System.Security.Cryptography;
using System.Text;
using CashFlow.Communication.Requests;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.PasswordResetTokens;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Services.Email;
using CashFlow.Exception.ExceptionsBase;
using CashFlow.Infrastructure.PasswordReset;

namespace CashFlow.Application.UseCases.Users.ForgotPassword
{
    public class ForgotPasswordUseCase : IForgotPasswordUseCase
    {
        private readonly IUserReadOnlyRepository _userRepository;
        private readonly IPasswordResetTokensWriteOnlyRepository _writeOnlyRepository;
        private readonly IPasswordResetTokensUpdateOnlyRepository _updateOnlyRepository;
        private readonly IEmailSender _emailSender;
        private readonly PasswordResetSettings _settings;
        private readonly IUnitOfWork _unitOfWork;

        public ForgotPasswordUseCase(
            IUserReadOnlyRepository userRepository,
            IPasswordResetTokensWriteOnlyRepository writeOnlyRepository,
            IPasswordResetTokensUpdateOnlyRepository updateOnlyRepository,
            IEmailSender emailSender,
            PasswordResetSettings settings,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _updateOnlyRepository = updateOnlyRepository;
            _emailSender = emailSender;
            _settings = settings;
            _unitOfWork = unitOfWork;
        }

        public async Task Execute(RequestForgotPassword request)
        {
            Validate(request);

            var user = await _userRepository.GetUserByEmail(request.Email.Trim());
            if (user is null)
            {
                return;
            }

            await _updateOnlyRepository.InvalidateActiveTokens(user.Id);

            var rawToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            var token = new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = ComputeSha256(rawToken),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_settings.TokenExpiresMinutes)
            };

            await _writeOnlyRepository.Add(token);
            await _unitOfWork.Commit();

            var resetLink = $"{_settings.BaseUrl}?token={rawToken}";
            var body = $"<p>Voce solicitou a redefinicao de senha.</p><p><a href=\"{resetLink}\">Redefinir senha</a></p>";

            await _emailSender.Send(user.Email, "CashFlow - Redefinicao de senha", body);
        }

        private static string ComputeSha256(string value)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
            return Convert.ToHexString(bytes);
        }

        private static void Validate(RequestForgotPassword request)
        {
            var validator = new ForgotPasswordValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                throw new ErrorOnValidationException(result.Errors.Select(error => error.ErrorMessage).ToList());
            }
        }
    }
}
