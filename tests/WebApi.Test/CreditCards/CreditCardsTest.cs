using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.CreditCards
{
    public class CreditCardsTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public CreditCardsTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Register_Credit_Card_And_Statement()
        {
            await Authenticate();
            await EnsureFinancialSetup();

            var cardRequest = RequestCreditCardBuilder.Build();
            var cardResponse = await _httpClient.PostAsJsonAsync("api/credit-cards", cardRequest);
            cardResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var cardResponseBody = await cardResponse.Content.ReadAsStreamAsync();
            var cardJson = await JsonDocument.ParseAsync(cardResponseBody);
            var cardId = cardJson.RootElement.GetProperty("id").GetInt64();

            var statementRequest = RequestCreditCardStatementBuilder.Build(cardId);
            var statementResponse = await _httpClient.PostAsJsonAsync("api/credit-cards/statements", statementRequest);
            statementResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var listResponse = await _httpClient.GetAsync("api/credit-cards/statements?month=2026-04-01");
            listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var listBody = await listResponse.Content.ReadAsStreamAsync();
            var listJson = await JsonDocument.ParseAsync(listBody);
            listJson.RootElement.GetProperty("statements").EnumerateArray().Should().HaveCount(1);
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
