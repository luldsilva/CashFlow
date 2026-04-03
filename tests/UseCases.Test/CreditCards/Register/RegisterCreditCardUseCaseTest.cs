using CashFlow.Application.UseCases.CreditCards.Register;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.CreditCards;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Moq;

namespace UseCases.Test.CreditCards.Register
{
    public class RegisterCreditCardUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = RequestCreditCardBuilder.Build();
            var writeRepository = new Mock<ICreditCardsWriteOnlyRepository>();
            var useCase = CreateUseCase(writeRepository.Object);

            var result = await useCase.Execute(request);

            result.Name.Should().Be(request.Name);
            result.LastFourDigits.Should().Be(request.LastFourDigits);
            writeRepository.Verify(repository => repository.Add(It.IsAny<CreditCard>()), Times.Once);
        }

        [Fact]
        public async Task Error_When_Household_Does_Not_Exist()
        {
            var request = RequestCreditCardBuilder.Build();
            var householdRepository = new Mock<IHouseholdReadOnlyRepository>();
            householdRepository.Setup(repository => repository.GetByUserId(It.IsAny<long>())).ReturnsAsync((Household?)null);

            var useCase = CreateUseCase(householdRepository: householdRepository.Object);

            var act = async () => await useCase.Execute(request);

            var result = await act.Should().ThrowAsync<NotFoundException>();
            result.Where(ex => ex.GetErrors().Contains("Financial setup must be created before managing credit cards."));
        }

        private static RegisterCreditCardUseCase CreateUseCase(
            ICreditCardsWriteOnlyRepository? repository = null,
            IHouseholdReadOnlyRepository? householdRepository = null)
        {
            var loggedUser = new Mock<ILoggedUser>();
            loggedUser.Setup(service => service.Get()).ReturnsAsync(new User { Id = 10 });

            var readRepository = householdRepository ?? Mock.Of<IHouseholdReadOnlyRepository>(
                repository => repository.GetByUserId(It.IsAny<long>()) == Task.FromResult<Household?>(new Household { Id = 1, UserId = 10 }));

            return new RegisterCreditCardUseCase(
                repository ?? Mock.Of<ICreditCardsWriteOnlyRepository>(),
                readRepository,
                loggedUser.Object,
                Mock.Of<IUnitOfWork>());
        }
    }
}
