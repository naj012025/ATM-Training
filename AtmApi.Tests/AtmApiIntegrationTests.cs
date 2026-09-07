using AtmApi.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace AtmApi.Tests;

public sealed class AtmApiIntegrationTests : IClassFixture<AtmApiFactory>
{
    private readonly AtmApiFactory _factory;
    private readonly HttpClient _client;

    public AtmApiIntegrationTests(AtmApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> LoginAsync(string accountNumber, string pin)
    {
        LoginRequest request = new()
        {
            AccountNumber = accountNumber,
            Pin = pin
        };

        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "api/auth/Login",
                request);
        response.EnsureSuccessStatusCode();

        LoginResponse? login =
            await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(login);
        Assert.False(string.IsNullOrWhiteSpace(login.AccessToken));

        return login.AccessToken;
    }

    private void UseBearer(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }





}
