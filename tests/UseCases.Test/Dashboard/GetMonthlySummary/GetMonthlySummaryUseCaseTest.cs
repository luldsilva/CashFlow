using CashFlow.Application.UseCases.Dashboard.GetMonthlySummary;
using CashFlow.Domain.Entities;
using CashFlow.Domain.Enums;
using CashFlow.Domain.Repositories.CreditCardStatements;
using CashFlow.Domain.Repositories.FinancialObligations;
using CashFlow.Domain.Repositories.Households;
using CashFlow.Domain.Repositories.MonthlyClosures;
using CashFlow.Domain.Services.LoggedUser;
using CashFlow.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace UseCases.Test.Dashboard.GetMonthlySummary
{
    public class GetMonthlySummaryUseCaseTest
    {
        [Fact]
        public async Task Success()
        {
            var competenceDate = new DateTime(2026, 4, 1);
            var household = new Household
            {
                Id = 1,
                UserId = 10,
                IncomeSources =
                [
                    new IncomeSource { Amount = 10000m },
                    new IncomeSource { Amount = 2000m }
                ],
                PlanningBuckets =
                [
                    new PlanningBucket { Code = "essentials", Name = "Essenciais", Percentage = 50m, IsActive = true, DisplayOrder = 0 },
                    new PlanningBucket { Code = "investments", Name = "Investimentos", Percentage = 20m, IsActive = true, DisplayOrder = 1 }
                ]
            };

            var obligations = new List<FinancialObligation>
            {
                new()
                {
                    Id = 1,
                    Title = "Aluguel",
                    CategoryName = "Moradia",
                    BucketCode = "essentials",
                    Amount = 3000m,
                    DueDate = new DateTime(2026, 4, 10),
                    CompetenceDate = competenceDate,
                    Status = FinancialObligationStatus.Expected
                },
                new()
                {
                    Id = 2,
                    Title = "Conta paga",
                    CategoryName = "Servicos",
                    BucketCode = "essentials",
                    Amount = 500m,
                    PaidAmount = 450m,
                    PaidDate = new DateTime(2026, 4, 5),
                    DueDate = new DateTime(2026, 4, 5),
                    CompetenceDate = competenceDate,
                    Status = FinancialObligationStatus.Paid
                },
                new()
                {
                    Id = 3,
                    Title = "Aporte planejado",
                    CategoryName = "Investimentos",
                    BucketCode = "investments",
                    Amount = 1000m,
                    DueDate = new DateTime(2026, 4, 20),
                    CompetenceDate = competenceDate,
                    Status = FinancialObligationStatus.Expected
                }
            };

            var useCase = CreateUseCase(household, obligations);

            var result = await useCase.Execute(competenceDate);

            result.PlannedIncome.Should().Be(12000m);
            result.PaidOutflow.Should().Be(450m);
            result.CommittedOutflow.Should().Be(4000m);
            result.FreeToSpend.Should().Be(7550m);
            result.FreeToInvest.Should().Be(1400m);
            result.Buckets.Should().HaveCount(2);
            result.UpcomingObligations.Should().HaveCount(2);
        }

        [Fact]
        public async Task Error_When_Household_Does_Not_Exist()
        {
            var householdRepository = new Mock<IHouseholdReadOnlyRepository>();
            householdRepository
                .Setup(repository => repository.GetByUserId(It.IsAny<long>()))
                .ReturnsAsync((Household?)null);

            var loggedUser = new Mock<ILoggedUser>();
            loggedUser.Setup(service => service.Get()).ReturnsAsync(new User { Id = 10 });

            var useCase = new GetMonthlySummaryUseCase(
                householdRepository.Object,
                Mock.Of<IFinancialObligationsReadOnlyRepository>(),
                Mock.Of<ICreditCardStatementsReadOnlyRepository>(repository =>
                    repository.GetByMonth(It.IsAny<long>(), It.IsAny<DateTime>()) == Task.FromResult(new List<CreditCardStatement>())),
                Mock.Of<IMonthlyClosuresReadOnlyRepository>(repository =>
                    repository.GetByMonth(It.IsAny<long>(), It.IsAny<DateTime>()) == Task.FromResult<MonthlyClosure?>(null)),
                loggedUser.Object);

            var act = async () => await useCase.Execute(new DateTime(2026, 4, 1));

            var result = await act.Should().ThrowAsync<NotFoundException>();
            result.Where(ex => ex.GetErrors().Contains("Financial setup must be created before viewing the monthly summary."));
        }

        private static GetMonthlySummaryUseCase CreateUseCase(Household household, List<FinancialObligation> obligations)
        {
            var householdRepository = new Mock<IHouseholdReadOnlyRepository>();
            householdRepository
                .Setup(repository => repository.GetByUserId(It.IsAny<long>()))
                .ReturnsAsync(household);

            var obligationsRepository = new Mock<IFinancialObligationsReadOnlyRepository>();
            obligationsRepository
                .Setup(repository => repository.GetAllByCompetence(It.IsAny<long>(), It.IsAny<DateTime>()))
                .ReturnsAsync(obligations);

            var loggedUser = new Mock<ILoggedUser>();
            loggedUser.Setup(service => service.Get()).ReturnsAsync(new User { Id = 10 });

            return new GetMonthlySummaryUseCase(
                householdRepository.Object,
                obligationsRepository.Object,
                Mock.Of<ICreditCardStatementsReadOnlyRepository>(repository =>
                    repository.GetByMonth(It.IsAny<long>(), It.IsAny<DateTime>()) == Task.FromResult(new List<CreditCardStatement>())),
                Mock.Of<IMonthlyClosuresReadOnlyRepository>(repository =>
                    repository.GetByMonth(It.IsAny<long>(), It.IsAny<DateTime>()) == Task.FromResult<MonthlyClosure?>(null)),
                loggedUser.Object);
        }
    }
}
