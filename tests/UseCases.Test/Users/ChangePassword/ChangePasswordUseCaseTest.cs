using CashFlow.Application.UseCases.Users.ChangePassword;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.User;
using CashFlow.Domain.Security.Cryptography;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Moq;

namespace UseCases.Test.Users.ChangePassword
{
    public class ChangePasswordUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var passwordEncripter = new Mock<IPasswordEncripter>();
            passwordEncripter.Setup(service => service.Verify("old123", "hash")).Returns(true);
            passwordEncripter.Setup(service => service.Encrypt(It.IsAny<string>())).Returns("new-hash");

            var user = new User { Id = 1, Password = "hash" };
            var repository = new Mock<IUserUpdateOnlyRepository>();
            repository.Setup(current => current.GetById(1)).ReturnsAsync(user);

            var loggedUser = new Mock<ILoggedUser>();
            loggedUser.Setup(service => service.Get()).ReturnsAsync(new User { Id = 1 });

            var request = new CashFlow.Communication.Requests.RequestChangePassword
            {
                CurrentPassword = "old123",
                NewPassword = "new12345",
                ConfirmNewPassword = "new12345"
            };

            var useCase = new ChangePasswordUseCase(repository.Object, passwordEncripter.Object, loggedUser.Object, Mock.Of<IUnitOfWork>());

            await useCase.Execute(request);

            user.Password.Should().Be("new-hash");
            repository.Verify(current => current.Update(user), Times.Once);
        }

        [Fact]
        public async Task Error_When_Current_Password_Is_Invalid()
        {
            var passwordEncripter = new Mock<IPasswordEncripter>();
            passwordEncripter.Setup(service => service.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var repository = new Mock<IUserUpdateOnlyRepository>();
            repository.Setup(current => current.GetById(1)).ReturnsAsync(new User { Id = 1, Password = "hash" });

            var loggedUser = new Mock<ILoggedUser>();
            loggedUser.Setup(service => service.Get()).ReturnsAsync(new User { Id = 1 });

            var request = new CashFlow.Communication.Requests.RequestChangePassword
            {
                CurrentPassword = "wrong",
                NewPassword = "new12345",
                ConfirmNewPassword = "new12345"
            };

            var useCase = new ChangePasswordUseCase(repository.Object, passwordEncripter.Object, loggedUser.Object, Mock.Of<IUnitOfWork>());

            var act = async () => await useCase.Execute(request);

            var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
            result.Where(ex => ex.GetErrors().Contains("Current password is invalid."));
        }
    }
}
