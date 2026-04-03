using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.FinancialObligations
{
    public class FinancialObligationsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public FinancialObligationsTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Register_And_List_By_Month()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var request = RequestFinancialObligationBuilder.Build();

            var registerResponse = await _httpClient.PostAsJsonAsync("api/financial-obligations", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var listResponse = await _httpClient.GetAsync("api/financial-obligations?month=2026-04-01");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await listResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var obligations = responseData.RootElement.GetProperty("obligations").EnumerateArray().ToList();

            obligations.Should().HaveCount(1);
            obligations[0].GetProperty("title").GetString().Should().Be(request.Title);
            obligations[0].GetProperty("categoryName").GetString().Should().Be(request.CategoryName);
            obligations[0].GetProperty("bucketCode").GetString().Should().Be(request.BucketCode);
        }

        private async Task EnsureFinancialSetup()
        {
            var request = RequestUpsertFinancialSetupBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("api/financial-setup", request);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
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
