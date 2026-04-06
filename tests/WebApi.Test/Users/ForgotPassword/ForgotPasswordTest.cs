using CashFlow.Communication.Requests;
using CashFlow.Infrastructure.Email;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace WebApi.Test.Users.ForgotPassword
{
    public class ForgotPasswordTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly CustomWebApplicationFactory _factory;

        public ForgotPasswordTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _factory = factory;
        }

        [Fact]
        public async Task Success()
        {
            var request = new RequestForgotPassword
            {
                Email = _factory.GetEmail()
            };

            var response = await _httpClient.PostAsJsonAsync("api/User/forgot-password", request);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var store = _factory.Services.GetService(typeof(PasswordResetEmailDebugStore)) as PasswordResetEmailDebugStore;
            store.Should().NotBeNull();
            store!.GetLatestLink(_factory.GetEmail()).Should().NotBeNullOrWhiteSpace();
        }
    }
}
