using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.FinancialSetup
{
    public class FinancialSetupTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public FinancialSetupTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Put_Then_Get_Financial_Setup()
        {
            await Authenticate();

            var request = RequestUpsertFinancialSetupBuilder.Build();

            var putResponse = await _httpClient.PutAsJsonAsync("api/financial-setup", request);
            putResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResponse = await _httpClient.GetAsync("api/financial-setup");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await getResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("householdName").GetString().Should().Be(request.HouseholdName);
            responseData.RootElement.GetProperty("membersCount").GetInt32().Should().Be(request.MembersCount);
            responseData.RootElement.GetProperty("incomeSources").EnumerateArray().Should().HaveCount(2);
            responseData.RootElement.GetProperty("planningBuckets").EnumerateArray().Should().HaveCount(3);
            responseData.RootElement.GetProperty("expenseCategories").EnumerateArray().Should().HaveCount(3);
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
