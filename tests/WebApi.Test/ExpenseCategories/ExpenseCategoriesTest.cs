using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.ExpenseCategories
{
    public class ExpenseCategoriesTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public ExpenseCategoriesTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Register_And_List_Expense_Categories()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var request = new RequestManageExpenseCategory
            {
                Name = "Moradia"
            };

            var registerResponse = await _httpClient.PostAsJsonAsync("api/expense-categories", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var registerBody = await registerResponse.Content.ReadAsStreamAsync();
            var registerJson = await JsonDocument.ParseAsync(registerBody);
            registerJson.RootElement.GetProperty("bucketCode").ValueKind.Should().Be(JsonValueKind.Null);
            registerJson.RootElement.GetProperty("bucketName").ValueKind.Should().Be(JsonValueKind.Null);

            var listResponse = await _httpClient.GetAsync("api/expense-categories");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await listResponse.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(body);
            json.RootElement.GetProperty("categories").EnumerateArray()
                .Should().Contain(item => item.GetProperty("name").GetString() == "Moradia");
        }

        private async Task EnsureFinancialSetup()
        {
            var request = RequestUpsertFinancialSetupBuilder.Build();
            var response = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Created, HttpStatusCode.Conflict);
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
