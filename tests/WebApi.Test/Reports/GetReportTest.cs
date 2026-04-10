using CashFlow.Communication.Requests;
using CashFlow.Exception;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.Reports
{
    public class GetReportTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _webApplicationFactory;

        public GetReportTest(CustomWebApplicationFactory webApplicationFactory)
        {
            _httpClient = webApplicationFactory.CreateClient();
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task GetExcel_Without_Data_Returns_NotFound()
        {
            await Authenticate();

            var response = await _httpClient.GetAsync("api/Report/excel?month=2030-01-01");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

            errors.Should().HaveCount(1).And.Contain(error =>
                error.GetString()!.Equals(ResourceErrorMessages.ResourceManager.GetString("REPORT_WITHOUT_DATA_FOR_MONTH")));
        }

        [Fact]
        public async Task GetPdf_Without_Data_Returns_NotFound()
        {
            await Authenticate();

            var response = await _httpClient.GetAsync("api/Report/pdf?month=2030-01-01");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var responseBody = await response.Content.ReadAsStreamAsync();
            var responseData = await JsonDocument.ParseAsync(responseBody);
            var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

            errors.Should().HaveCount(1).And.Contain(error =>
                error.GetString()!.Equals(ResourceErrorMessages.ResourceManager.GetString("REPORT_WITHOUT_DATA_FOR_MONTH")));
        }

        [Fact]
        public async Task GetExcel_With_Data_Returns_File()
        {
            await Authenticate();

            var request = RequestRegisterExpenseBuilder.Build();
            request.Date = new DateTime(2024, 8, 10, 10, 0, 0);

            var registerResponse = await _httpClient.PostAsJsonAsync("api/Expenses", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var response = await _httpClient.GetAsync("api/Report/excel?month=2024-08-01");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var file = await response.Content.ReadAsByteArrayAsync();
            file.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetExcel_Should_Not_Return_Another_User_Expenses()
        {
            await Authenticate();

            var request = RequestRegisterExpenseBuilder.Build();
            request.Date = new DateTime(2024, 9, 10, 10, 0, 0);

            var registerResponse = await _httpClient.PostAsJsonAsync("api/Expenses", request);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            using var secondClient = _webApplicationFactory.CreateClient();
            var secondUser = RequestRegisterUserBuilder.Build();

            var registerUserResponse = await secondClient.PostAsJsonAsync("api/User", secondUser);
            registerUserResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var loginResponse = await secondClient.PostAsJsonAsync("api/Login", new RequestLogin
            {
                Email = secondUser.Email,
                Password = secondUser.Password
            });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginBody = await loginResponse.Content.ReadAsStreamAsync();
            var loginJson = await JsonDocument.ParseAsync(loginBody);
            var token = loginJson.RootElement.GetProperty("token").GetString();

            secondClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await secondClient.GetAsync("api/Report/excel?month=2024-09-01");

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
