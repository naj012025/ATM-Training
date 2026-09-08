using AtmApi.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace AtmApi.Tests;

public abstract class AtmApiIntegrationTestBase
{
    protected readonly AtmApiFactory _factory;
    protected readonly HttpClient _client;

    protected AtmApiIntegrationTestBase(AtmApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task<string> LoginAsync(string accountNumber, string pin)
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

    protected void UseBearer(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }





}
