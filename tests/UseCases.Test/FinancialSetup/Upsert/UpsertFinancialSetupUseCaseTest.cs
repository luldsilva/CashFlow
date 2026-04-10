using CashFlow.Application.UseCases.FinancialSetup.Register;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Repositories;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using FluentAssertions;
using Moq;

namespace UseCases.Test.FinancialSetup.Upsert
{
    public class UpsertFinancialSetupUseCaseTest
    {
        [Fact]
        public async Task Success_Create_Financial_Setup()
        {
            var request = RequestUpsertFinancialSetupBuilder.Build();
            var writeRepository = new Mock<IHouseholdWriteOnlyRepository>();
            var unitOfWork = UnitOfWorkBuilder.Build();
            var useCase = CreateUseCase(writeRepository.Object, unitOfWork);

            var result = await useCase.Execute(request);

            result.Should().NotBeNull();
            result.HouseholdName.Should().Be(request.HouseholdName);
            result.MembersCount.Should().Be(request.MembersCount);
            result.IncomeSources.Should().HaveCount(request.IncomeSources.Count);
            result.PlanningBuckets.Should().HaveCount(request.PlanningBuckets.Count);
            result.ExpenseCategories.Should().HaveCount(request.ExpenseCategories.Count);

            writeRepository.Verify(repository => repository.Add(It.IsAny<Household>()), Times.Once);
        }

        [Fact]
        public async Task Error_When_Active_Buckets_Do_Not_Sum_100()
        {
            var request = RequestUpsertFinancialSetupBuilder.Build();
            request.PlanningBuckets[0].Percentage = 40m;

            var useCase = CreateUseCase();

            var act = async () => await useCase.Execute(request);

            var result = await act.Should().ThrowAsync<ErrorOnValidationException>();

            result.Where(ex => ex.GetErrors().Contains("Active planning buckets must sum 100%."));
        }

        private static RegisterFinancialSetupUseCase CreateUseCase(
            IHouseholdWriteOnlyRepository? writeRepository = null,
            IUnitOfWork? unitOfWork = null)
        {
            var readRepository = new Mock<IHouseholdReadOnlyRepository>();
            readRepository
                .Setup(repository => repository.GetByUserId(It.IsAny<long>()))
                .ReturnsAsync((Household?)null);

            var loggedUser = new Mock<ILoggedUser>();
            loggedUser
                .Setup(service => service.Get())
                .ReturnsAsync(new User { Id = 99 });

            return new RegisterFinancialSetupUseCase(
                readRepository.Object,
                writeRepository ?? Mock.Of<IHouseholdWriteOnlyRepository>(),
                loggedUser.Object,
                unitOfWork ?? Mock.Of<IUnitOfWork>());
        }
    }
}
