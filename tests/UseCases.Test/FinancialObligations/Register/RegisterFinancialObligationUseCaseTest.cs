using AutoMapper;
using CashFlow.Application.AutoMapper;
using CashFlow.Application.UseCases.FinancialObligations.Register;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Moq;

namespace UseCases.Test.FinancialObligations.Register
{
    public class RegisterFinancialObligationUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var request = RequestFinancialObligationBuilder.Build();
            var repository = new Mock<IFinancialObligationsWriteOnlyRepository>();
            var useCase = CreateUseCase(repository.Object);

            var result = await useCase.Execute(request);

            result.Should().NotBeNull();
            result.Title.Should().Be(request.Title);
            result.CategoryName.Should().Be(request.CategoryName);
            result.Amount.Should().Be(request.Amount);
            result.CompetenceDate.Should().Be(new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc));

            repository.Verify(current => current.Add(It.IsAny<FinancialObligation>()), Times.Once);
        }

        [Fact]
        public async Task Error_When_Household_Does_Not_Exist()
        {
            var request = RequestFinancialObligationBuilder.Build();
            var repository = new Mock<IHouseholdReadOnlyRepository>();
            repository
                .Setup(current => current.GetByUserId(It.IsAny<long>()))
                .ReturnsAsync((Household?)null);

            var useCase = CreateUseCase(householdRepository: repository.Object);

            var act = async () => await useCase.Execute(request);

            var result = await act.Should().ThrowAsync<NotFoundException>();
            result.Where(ex => ex.GetErrors().Contains("Financial setup must be created before managing monthly obligations."));
        }

        [Fact]
        public async Task Error_When_Paid_Obligation_Misses_Paid_Information()
        {
            var request = RequestFinancialObligationBuilder.Build();
            request.Status = CashFlow.Communication.Enums.FinancialObligationStatus.Paid;

            var useCase = CreateUseCase();

            var act = async () => await useCase.Execute(request);

            var result = await act.Should().ThrowAsync<ErrorOnValidationException>();
            result.Where(ex => ex.GetErrors().Contains("Paid obligations must include paid amount and paid date."));
        }

        private static RegisterFinancialObligationUseCase CreateUseCase(
            IFinancialObligationsWriteOnlyRepository? repository = null,
            IHouseholdReadOnlyRepository? householdRepository = null)
        {
            var mapper = new MapperConfiguration(configuration => configuration.AddProfile(new AutoMapping())).CreateMapper();

            var loggedUser = new Mock<ILoggedUser>();
            loggedUser.Setup(service => service.Get()).ReturnsAsync(new User { Id = 10 });

            var householdReadOnlyRepository = householdRepository ?? Mock.Of<IHouseholdReadOnlyRepository>(current =>
                current.GetByUserId(It.IsAny<long>()) == Task.FromResult<Household?>(new Household { Id = 20, UserId = 10 }));

            return new RegisterFinancialObligationUseCase(
                mapper,
                repository ?? Mock.Of<IFinancialObligationsWriteOnlyRepository>(),
                householdReadOnlyRepository,
                loggedUser.Object,
                Mock.Of<IUnitOfWork>());
        }
    }
}
