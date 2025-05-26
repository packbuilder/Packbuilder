using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Packbuilder.Config;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Services
{
    public class BrevoEmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly AppSettings _appSettings;

        public BrevoEmailService(HttpClient httpClient, IOptions<BrevoSettings> brevoSettings, IOptions<AppSettings> appSettings)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(brevoSettings.Value.BaseUrl);
            _apiKey = brevoSettings.Value.ApiKey;
            _appSettings = appSettings.Value;
        }

        public async Task SendVerificationEmail(string email, string token, int userId)
        {
            HttpRequestMessage request = new(HttpMethod.Post, "smtp/email");
            request.Headers.Add("api-key", _apiKey);
            string verifyUrl = $"{_appSettings.BackendBaseUrl}/verification?verificationCode={token}&userId={userId}";

            var body = new
            {
                sender = new {
                    email = "no-reply@packbuilder.org",
                    name = "Packbuilder"
                },
                to = new[] { new { email } },
                subject = "Verify your email for Packbuilder.",
                htmlContent = $@"
                    <h1>Click below to verify:</h1>
                    <a href=""{verifyUrl}"">Verify Email</a>
                    <p>This link will expire 24 hours after its send date. If you request another email to be sent at some point, this link will no longer be valid.</p>
                "
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Brevo email failed: {response.StatusCode} - {error}");
                throw new Exception($"Brevo email failed: {response.StatusCode} - {error}");
            }
        }

        public async Task SendPasswordResetEmail(string email, string token, int userId)
        {
            HttpRequestMessage request = new(HttpMethod.Post, "smtp/email");
            request.Headers.Add("api-key", _apiKey);
            string verifyUrl = $"{_appSettings.FrontendBaseUrl}/profile/reset-password?token={token}&userId={userId}";

            var body = new
            {
                sender = new {
                    email = "no-reply@packbuilder.org",
                    name = "Packbuilder"
                },
                to = new[] { new { email } },
                subject = "Reset password for your Packbuilder account.",
                htmlContent = $@"
                    <h1>Click below to reset your password:</h1>
                    <a href=""{verifyUrl}"">Reset password</a>
                    <p>You will be redirected to packbuilder to change your password. This link is only valid for 15 minutes. If you request another link this link will no longer be valid.</p>
                "
            };

            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Brevo email failed: {response.StatusCode} - {error}");
                throw new Exception($"Brevo email failed: {response.StatusCode} - {error}");
            }
        }
    }
}