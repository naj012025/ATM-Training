using AtmApi.Data;
using AtmApi.DTO;
using AtmApi.Models;
using AtmApi.Tests;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace AtmApi.Tests;

//learned i can use the iclassfixture with a diffrent name and it works i wanted to make a seperate file for the
//Arrange Act Asserts easier to read.
public sealed class AccountTests :
    AtmApiIntegrationTestBase,
    IClassFixture<AtmApiFactory>
{
    private readonly AtmApiFactory _factory;
    private readonly HttpClient _client;

    public AccountTests(AtmApiFactory factory) : base(factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithCorrectPin_ReturnsToken()
    {
        //Arrange
        await _factory.SeedAccountAsync(
            "20001",
            "1234",
            1000m);
        LoginRequest request = new()
        {
            AccountNumber = "20001",
            Pin = "1234"
        };

        //Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "/api/auth/Login",
                request);
        //Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
        LoginResponse? body =
            await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body.AccessToken));

    }

    [Fact]
    public async Task Login_WithPin_ReturnsUnauthorized()
    {
        //Arrange
        await _factory.SeedAccountAsync(
            "20001",
            "1234",
            1000m);
        LoginRequest request = new()
        {
            AccountNumber = "20001",
            Pin = "4567" //wrong Pin.
        };
        //Act
        HttpResponseMessage response =
            await _client.PostAsJsonAsync(
                "api/auth/Login",
                request);
        //Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    //Regression test will fail if:
    // breaking authentication registration,
    // or changing middleware/security
    public async Task GetMe_WithoutToken_ReturnsUnauthorized()
    {
        //Arrange
        //Noarrange just act/assert.
        //Act
        HttpResponseMessage response =
            await _client.GetAsync(
                "api/accounts/me");
        //Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task GetMe_WithValidToken_ReturnsValidAccount()
    {
        //Arrange
        await _factory.SeedAccountAsync(
            "20003",
            "1234",
            1000m);
        string token =
            await LoginAsync(
                "20003", "1234");
        UseBearer(token);
        //Act

        //Assert
    }


}