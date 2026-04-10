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
            request.CompetenceDate = new DateTime(2026, 7, 1);
            request.DueDate = new DateTime(2026, 7, 9);

            var registerResponse = await _httpClient.PostAsJsonAsync("api/financial-obligations", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var listResponse = await _httpClient.GetAsync("api/financial-obligations?month=2026-07-01");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await listResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var obligations = responseData.RootElement.GetProperty("obligations").EnumerateArray().ToList();

            obligations.Should().ContainSingle(obligation => obligation.GetProperty("title").GetString() == request.Title);
            var obligation = obligations.Single(item => item.GetProperty("title").GetString() == request.Title);
            obligation.GetProperty("categoryName").GetString().Should().Be(request.CategoryName);
            obligation.GetProperty("bucketCode").GetString().Should().Be(request.BucketCode);
        }

        [Fact]
        public async Task Register_Should_Accept_Obligation_Without_Bucket_Code()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var request = RequestFinancialObligationBuilder.Build();
            request.BucketCode = null;
            request.CompetenceDate = new DateTime(2026, 8, 1);
            request.DueDate = new DateTime(2026, 8, 12);

            var registerResponse = await _httpClient.PostAsJsonAsync("api/financial-obligations", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseBody = await registerResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("bucketCode").GetString().Should().BeEmpty();
            responseData.RootElement.GetProperty("requiresReview").GetBoolean().Should().BeTrue();
        }

        [Fact]
        public async Task Register_Should_Link_Official_Category()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var categoryResponse = await _httpClient.PostAsJsonAsync("api/expense-categories", new RequestManageExpenseCategory
            {
                Name = "Transporte"
            });
            categoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var categoryBody = await categoryResponse.Content.ReadAsStreamAsync();
            var categoryJson = await JsonDocument.ParseAsync(categoryBody);
            var categoryId = categoryJson.RootElement.GetProperty("id").GetInt64();

            var request = RequestFinancialObligationBuilder.Build();
            request.CategoryId = categoryId;
            request.CategoryName = "Transporte";
            request.CompetenceDate = new DateTime(2026, 9, 1);
            request.DueDate = new DateTime(2026, 9, 8);

            var registerResponse = await _httpClient.PostAsJsonAsync("api/financial-obligations", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseBody = await registerResponse.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);

            responseData.RootElement.GetProperty("categoryId").GetInt64().Should().Be(categoryId);
            responseData.RootElement.GetProperty("categoryName").GetString().Should().Be("Transporte");
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
