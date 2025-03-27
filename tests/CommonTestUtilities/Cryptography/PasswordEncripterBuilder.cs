using CashFlow.Domain.Security.Cryptography;
using Moq;

namespace CommonTestUtilities.Cryptography
{
    public class PasswordEncripterBuilder
    {
        public static IPasswordEncripter Build()
        {
            var mock = new Mock<IPasswordEncripter>();

            mock.Setup(p => p.Encrypt(It.IsAny<string>())).Returns("!%dlfjkd545");

            return mock.Object;
        }
    }
}
