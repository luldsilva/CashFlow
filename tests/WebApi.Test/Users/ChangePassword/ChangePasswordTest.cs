using CashFlow.Communication.Requests;
using CommonTestUtilities.Requests;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Test.Users.ChangePassword
{
    public class ChangePasswordTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public ChangePasswordTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task Success()
        {
            await Authenticate(_factory.GetPassword());

            var request = RequestChangePasswordBuilder.Build(_factory.GetPassword());

            var changeResponse = await _httpClient.PostAsJsonAsync("api/User/change-password", request);
            changeResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            _httpClient.DefaultRequestHeaders.Authorization = null;

            var loginWithNewPassword = new RequestLogin
            {
                Email = _factory.GetEmail(),
                Password = request.NewPassword
            };

            var loginResponse = await _httpClient.PostAsJsonAsync("api/Login", loginWithNewPassword);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task Authenticate(string password)
        {
            var login = new RequestLogin
            {
                Email = _factory.GetEmail(),
                Password = password
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
