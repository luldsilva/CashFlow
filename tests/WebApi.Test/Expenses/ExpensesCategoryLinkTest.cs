using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.Expenses
{
    public class ExpensesCategoryLinkTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public ExpensesCategoryLinkTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Register_Expense_Should_Link_Official_Category()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var categoryId = await CreateCategory("Mercado Premium");

            var request = RequestRegisterExpenseBuilder.Build();
            request.CategoryId = categoryId;
            request.CategoryName = "Mercado Premium";

            var registerResponse = await _httpClient.PostAsJsonAsync("api/Expenses", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var getAllResponse = await _httpClient.GetAsync("api/Expenses");
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await getAllResponse.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(body);
            var expense = json.RootElement.GetProperty("expenses").EnumerateArray().First();

            expense.GetProperty("categoryId").GetInt64().Should().Be(categoryId);
            expense.GetProperty("categoryName").GetString().Should().Be("Mercado Premium");
        }

        private async Task<long> CreateCategory(string name)
        {
            var response = await _httpClient.PostAsJsonAsync("api/expense-categories", new RequestManageExpenseCategory
            {
                Name = name
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var body = await response.Content.ReadAsStreamAsync();
            var json = await JsonDocument.ParseAsync(body);
            return json.RootElement.GetProperty("id").GetInt64();
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
