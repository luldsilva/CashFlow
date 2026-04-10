using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.Dashboard
{
    public class DashboardTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public DashboardTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Get_Monthly_Summary()
        {
            await Authenticate();
            await EnsureFinancialSetup();
            await SeedFinancialObligations(new DateTime(2026, 9, 1));

            var response = await _httpClient.GetAsync("api/dashboard/monthly-summary?month=2026-09-01");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("plannedIncome").GetDecimal().Should().Be(13500m);
            responseData.RootElement.GetProperty("paidOutflow").GetDecimal().Should().Be(1200m);
            responseData.RootElement.GetProperty("committedOutflow").GetDecimal().Should().Be(1800m);
            responseData.RootElement.GetProperty("commitmentPercentage").GetDecimal().Should().BeApproximately(22.22m, 0.01m);
            responseData.RootElement.GetProperty("freeToSpend").GetDecimal().Should().Be(10500m);
            responseData.RootElement.GetProperty("buckets").EnumerateArray().Should().NotBeEmpty();
            responseData.RootElement.GetProperty("upcomingObligations").EnumerateArray().Should().HaveCount(1);
        }

        [Fact]
        public async Task Get_Monthly_Summary_Should_Suggest_Buckets_When_Planning_Is_Not_Configured()
        {
            await Authenticate();
            await EnsureFinancialSetup(withPlanning: false);
            await SeedFinancialObligations(new DateTime(2026, 10, 1));

            var response = await _httpClient.GetAsync("api/dashboard/monthly-summary?month=2026-10-01");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("buckets").EnumerateArray().Should().BeEmpty();
            responseData.RootElement.GetProperty("suggestedBuckets").EnumerateArray().Should().HaveCount(3);
        }

        [Fact]
        public async Task Get_Monthly_Summary_Should_Include_Credit_Card_Statements_In_Commitment()
        {
            await Authenticate();
            await EnsureFinancialSetup();
            await SeedFinancialObligations(new DateTime(2026, 11, 1));

            var creditCardRequest = RequestCreditCardBuilder.Build();
            creditCardRequest.Name = "Visa principal";
            creditCardRequest.ClosingDay = 25;
            creditCardRequest.DueDay = 5;

            var creditCardResponse = await _httpClient.PostAsJsonAsync("api/credit-cards", creditCardRequest);
            creditCardResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var creditCardJson = await JsonDocument.ParseAsync(await creditCardResponse.Content.ReadAsStreamAsync());
            var creditCardId = creditCardJson.RootElement.GetProperty("id").GetInt64();

            var statementRequest = RequestCreditCardStatementBuilder.Build(creditCardId);
            statementRequest.CompetenceDate = new DateTime(2026, 11, 1);
            statementRequest.ClosingDate = new DateTime(2026, 11, 25);
            statementRequest.DueDate = new DateTime(2026, 11, 5);
            statementRequest.TotalAmount = 700m;
            statementRequest.Status = CreditCardStatementStatus.Closed;

            var statementResponse = await _httpClient.PostAsJsonAsync("api/credit-cards/statements", statementRequest);
            statementResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var response = await _httpClient.GetAsync("api/dashboard/monthly-summary?month=2026-11-01");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("creditCardCommittedOutflow").GetDecimal().Should().Be(700m);
            responseData.RootElement.GetProperty("committedOutflow").GetDecimal().Should().Be(2500m);
            responseData.RootElement.GetProperty("creditCardStatements").EnumerateArray().Should().HaveCount(1);
        }

        private async Task EnsureFinancialSetup(bool withPlanning = true)
        {
            var request = RequestUpsertFinancialSetupBuilder.Build();
            if (!withPlanning)
            {
                request.PlanningBuckets = [];
                request.ExpenseCategories = [];
            }

            var response = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                if (!withPlanning)
                {
                    var deleteResponse = await _httpClient.DeleteAsync("api/financial-setup");
                    deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

                    var recreateResponse = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
                    recreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);
                    return;
                }

                var updateResponse = await _httpClient.PutAsJsonAsync("api/financial-setup", request);
                updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                return;
            }

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        private async Task SeedFinancialObligations(DateTime competenceDate)
        {
            var expected = RequestFinancialObligationBuilder.Build();
            expected.Amount = 1800m;
            expected.Status = FinancialObligationStatus.Expected;
            expected.Title = "Internet e utilidades";
            expected.CompetenceDate = competenceDate;
            expected.DueDate = competenceDate.AddDays(10);

            var paid = RequestFinancialObligationBuilder.Build();
            paid.Amount = 1200m;
            paid.PaidAmount = 1200m;
            paid.PaidDate = competenceDate.AddDays(5);
            paid.Status = FinancialObligationStatus.Paid;
            paid.Title = "Aluguel pago";
            paid.BucketCode = "essentials";
            paid.CompetenceDate = competenceDate;
            paid.DueDate = competenceDate.AddDays(5);

            var response1 = await _httpClient.PostAsJsonAsync("api/financial-obligations", expected);
            response1.StatusCode.Should().Be(HttpStatusCode.Created);

            var response2 = await _httpClient.PostAsJsonAsync("api/financial-obligations", paid);
            response2.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        private async Task Authenticate()
        {
            var login = new RequestLogin
            {
                Email = _webApplicationFactory.GetEmail(),
                Password = _webApplicationFactory.GetPassword()
            };

            var response = await _httpClient.PostAsJsonAsync("api/Login", login);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var token = responseData.RootElement.GetProperty("token").GetString();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
