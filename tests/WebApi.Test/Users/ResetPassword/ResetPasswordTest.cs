using CashFlow.Communication.Requests;
using CashFlow.Infrastructure.Email;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Test.Users.ResetPassword
{
    public class ResetPasswordTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public ResetPasswordTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task Success()
        {
            var forgotResponse = await _httpClient.PostAsJsonAsync("api/User/forgot-password", new RequestForgotPassword
            {
                Email = _factory.GetEmail()
            });

            forgotResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var store = _factory.Services.GetService(typeof(PasswordResetEmailDebugStore)) as PasswordResetEmailDebugStore;
            var link = store!.GetLatestLink(_factory.GetEmail());
            link.Should().NotBeNullOrWhiteSpace();

            var token = new Uri(link!).Query.Replace("?token=", string.Empty);
            var newPassword = "NovaSenha123";

            var resetResponse = await _httpClient.PostAsJsonAsync("api/User/reset-password", new RequestResetPassword
            {
                Token = token,
                NewPassword = newPassword,
                ConfirmNewPassword = newPassword
            });

            resetResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var loginResponse = await _httpClient.PostAsJsonAsync("api/Login", new RequestLogin
            {
                Email = _factory.GetEmail(),
                Password = newPassword
            });

            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
