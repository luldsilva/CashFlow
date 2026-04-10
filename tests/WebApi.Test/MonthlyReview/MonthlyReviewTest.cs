using CashFlow.Communication.Enums;
using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.MonthlyReview
{
    public class MonthlyReviewTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public MonthlyReviewTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Close_And_Get_Monthly_Review()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var obligation = RequestFinancialObligationBuilder.Build();
            obligation.Amount = 1000m;
            obligation.PaidAmount = 1000m;
            obligation.PaidDate = new DateTime(2026, 4, 4);
            obligation.Status = FinancialObligationStatus.Paid;

            var obligationResponse = await _httpClient.PostAsJsonAsync("api/financial-obligations", obligation);
            obligationResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var closeRequest = new RequestCloseMonth
            {
                CompetenceDate = new DateTime(2026, 4, 1),
                Notes = "Mes fechado com contas principais quitadas."
            };

            var closeResponse = await _httpClient.PostAsJsonAsync("api/monthly-review/close", closeRequest);
            closeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResponse = await _httpClient.GetAsync("api/monthly-review?month=2026-04-01");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await getResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            responseData.RootElement.GetProperty("closure").GetProperty("notes").GetString()
                .Should().Be(closeRequest.Notes);
        }

        [Fact]
        public async Task Prepare_Should_Copy_Recurring_Obligations_To_Target_Month()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var obligation = RequestFinancialObligationBuilder.Build();
            obligation.Title = "Aluguel";
            obligation.CategoryName = "Moradia";
            obligation.CompetenceDate = new DateTime(2026, 12, 1);
            obligation.DueDate = new DateTime(2026, 12, 10);
            obligation.RecurrenceType = FinancialObligationRecurrenceType.FixedMonthly;
            obligation.Status = FinancialObligationStatus.Expected;

            var obligationResponse = await _httpClient.PostAsJsonAsync("api/financial-obligations", obligation);
            obligationResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var prepareRequest = new RequestPrepareMonth
            {
                TargetCompetenceDate = new DateTime(2027, 1, 1),
                SourceCompetenceDate = new DateTime(2026, 12, 1)
            };

            var prepareResponse = await _httpClient.PostAsJsonAsync("api/monthly-review/prepare", prepareRequest);
            prepareResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var listResponse = await _httpClient.GetAsync("api/financial-obligations?month=2027-01-01");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await listResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var obligations = responseData.RootElement.GetProperty("obligations").EnumerateArray().ToList();
            var clonedObligation = obligations.Single(item => item.GetProperty("title").GetString() == "Aluguel");
            clonedObligation.GetProperty("status").GetString().Should().Be("Expected");
        }

        private async Task EnsureFinancialSetup()
        {
            var request = RequestUpsertFinancialSetupBuilder.Build();
            var response = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var updateResponse = await _httpClient.PutAsJsonAsync("api/financial-setup", request);
                updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
                return;
            }

            response.StatusCode.Should().Be(HttpStatusCode.Created);
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
