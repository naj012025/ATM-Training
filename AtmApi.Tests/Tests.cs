using AtmApi.DTO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace AtmApi.Tests;

//learned i can use the iclassfixture with a diffrent name and it works i wanted to make a seperate file for the
//ArrangeactAsserts easier to read.
public sealed class Tests : IClassFixture<AtmApiFactory>
{
    private readonly AtmApiFactory _factory;
    private readonly HttpClient _client;

    public Tests(AtmApiFactory factory)
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


}