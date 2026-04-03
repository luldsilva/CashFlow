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
            await SeedFinancialObligations();

            var response = await _httpClient.GetAsync("api/dashboard/monthly-summary?month=2026-04-01");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("plannedIncome").GetDecimal().Should().Be(13500m);
            responseData.RootElement.GetProperty("paidOutflow").GetDecimal().Should().Be(1200m);
            responseData.RootElement.GetProperty("committedOutflow").GetDecimal().Should().Be(1800m);
            responseData.RootElement.GetProperty("freeToSpend").GetDecimal().Should().Be(10500m);
            responseData.RootElement.GetProperty("buckets").EnumerateArray().Should().NotBeEmpty();
            responseData.RootElement.GetProperty("upcomingObligations").EnumerateArray().Should().HaveCount(1);
        }

        private async Task EnsureFinancialSetup()
        {
            var request = RequestUpsertFinancialSetupBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("api/financial-setup", request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task SeedFinancialObligations()
        {
            var expected = RequestFinancialObligationBuilder.Build();
            expected.Amount = 1800m;
            expected.Status = FinancialObligationStatus.Expected;
            expected.Title = "Internet e utilidades";

            var paid = RequestFinancialObligationBuilder.Build();
            paid.Amount = 1200m;
            paid.PaidAmount = 1200m;
            paid.PaidDate = new DateTime(2026, 4, 5);
            paid.Status = FinancialObligationStatus.Paid;
            paid.Title = "Aluguel pago";
            paid.BucketCode = "essentials";

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
