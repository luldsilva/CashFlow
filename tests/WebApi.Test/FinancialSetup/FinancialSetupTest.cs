using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace WebApi.Test.FinancialSetup
{
    public class FinancialSetupTest
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public FinancialSetupTest()
        {
            _webApplicationFactory = new CustomWebApplicationFactory();
            _httpClient = _webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task Put_Then_Get_Financial_Setup()
        {
            await Authenticate();

            var request = RequestUpsertFinancialSetupBuilder.Build();

            var postResponse = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

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

        [Fact]
        public async Task Post_Financial_Setup_Should_Accept_Minimal_Onboarding_Without_Planning_Buckets()
        {
            await Authenticate();

            var request = RequestUpsertFinancialSetupBuilder.Build();
            request.PlanningBuckets = [];
            request.ExpenseCategories = [];

            var response = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("hasPlanningConfigured").GetBoolean().Should().BeFalse();
            responseData.RootElement.GetProperty("hasExpenseCategoriesConfigured").GetBoolean().Should().BeFalse();
            responseData.RootElement.GetProperty("planningBuckets").EnumerateArray().Should().BeEmpty();
            responseData.RootElement.GetProperty("expenseCategories").EnumerateArray().Should().BeEmpty();
        }

        [Fact]
        public async Task Put_Financial_Setup_Should_Accept_String_Enums()
        {
            await Authenticate();

            const string request =
                """
                {
                  "householdName": "Familia Silva Teste",
                  "membersCount": 3,
                  "hasVariableIncome": true,
                  "primaryIncomeFrequency": "Variable",
                  "planningModel": "Balanced",
                  "incomeSources": [
                    {
                      "name": "Renda principal",
                      "type": "Business",
                      "amount": 13000,
                      "isRecurring": true,
                      "frequency": "Monthly",
                      "expectedDayOfMonth": 10
                    }
                  ],
                  "planningBuckets": [
                    {
                      "name": "Essenciais",
                      "percentage": 50,
                      "isActive": true,
                      "displayOrder": 1
                    },
                    {
                      "name": "Educacao",
                      "percentage": 10,
                      "isActive": true,
                      "displayOrder": 2
                    },
                    {
                      "name": "Investimentos",
                      "percentage": 20,
                      "isActive": true,
                      "displayOrder": 3
                    },
                    {
                      "name": "Aposentadoria",
                      "percentage": 10,
                      "isActive": true,
                      "displayOrder": 4
                    },
                    {
                      "name": "Livre",
                      "percentage": 10,
                      "isActive": true,
                      "displayOrder": 5
                    }
                  ],
                  "expenseCategories": [
                    {
                      "name": "Aluguel",
                      "bucketName": "Essenciais"
                    },
                    {
                      "name": "Condominio",
                      "bucketName": "Essenciais"
                    },
                    {
                      "name": "Investimentos mensais",
                      "bucketName": "Investimentos"
                    }
                  ]
                }
                """;

            using var content = new StringContent(request, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/financial-setup", content);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Post_Financial_Setup_Should_Return_Conflict_When_Setup_Already_Exists()
        {
            await Authenticate();

            var request = RequestUpsertFinancialSetupBuilder.Build();

            var firstResponse = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var secondResponse = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Put_Financial_Setup_Should_Return_NotFound_When_Setup_Does_Not_Exist()
        {
            await Authenticate();

            var request = RequestUpsertFinancialSetupBuilder.Build();

            var response = await _httpClient.PutAsJsonAsync("api/financial-setup", request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_Financial_Setup_Should_Update_Existing_Setup()
        {
            await Authenticate();

            var request = RequestUpsertFinancialSetupBuilder.Build();
            var createResponse = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            request.HouseholdName = "Familia Silva Atualizada";
            request.PrimaryIncomeFrequency = CashFlow.Communication.Enums.HouseholdIncomeFrequency.Variable;

            var updateResponse = await _httpClient.PutAsJsonAsync("api/financial-setup", request);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var getResponse = await _httpClient.GetAsync("api/financial-setup");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await getResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("householdName").GetString().Should().Be("Familia Silva Atualizada");
            responseData.RootElement.GetProperty("primaryIncomeFrequency").GetString().Should().Be("Variable");
        }

        [Fact]
        public async Task Delete_Financial_Setup_Should_Remove_Existing_Setup()
        {
            await Authenticate();

            var request = RequestUpsertFinancialSetupBuilder.Build();
            var createResponse = await _httpClient.PostAsJsonAsync("api/financial-setup", request);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var deleteResponse = await _httpClient.DeleteAsync("api/financial-setup");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await _httpClient.GetAsync("api/financial-setup");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_Financial_Setup_Should_Return_NotFound_When_Setup_Does_Not_Exist()
        {
            await Authenticate();

            var response = await _httpClient.DeleteAsync("api/financial-setup");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
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
