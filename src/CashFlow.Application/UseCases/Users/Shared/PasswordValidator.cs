using CashFlow.Exception;
using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace CashFlow.Application.UseCases.Users.Shared
{
    public partial class PasswordValidator<T> : PropertyValidator<T, string>
    {
        private const string ERROR_MESSAGE_KEY = "ErrorMessage";
        public override string Name => "PasswordValidator";

        protected override string GetDefaultMessageTemplate(string errorCode)
        {
            return $"{{{ERROR_MESSAGE_KEY}}}";
        }

        public override bool IsValid(ValidationContext<T> context, string password)
        {
            if (UpperCaseLetter().IsMatch(password) == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if (LowerCaseLetter().IsMatch(password) == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if (Numbers().IsMatch(password) == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            if (SpecialSymbols().IsMatch(password) == false)
            {
                context.MessageFormatter.AppendArgument(ERROR_MESSAGE_KEY, ResourceErrorMessages.INVALID_PASSWORD);
                return false;
            }

            return true;
        }

        [GeneratedRegex(@"[A-Z]+")]
        public static partial Regex UpperCaseLetter();

        [GeneratedRegex(@"[a-z]+")]
        public static partial Regex LowerCaseLetter();

        [GeneratedRegex(@"[0-9]+")]
        public static partial Regex Numbers();

        [GeneratedRegex(@"[\!\?\*]+")]
        public static partial Regex SpecialSymbols();
    }
}
