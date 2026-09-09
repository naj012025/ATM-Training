using AtmApi.Data;
using AtmApi.DTO;
using AtmApi.Models;
using AtmApi.Tests;
using Microsoft.AspNetCore.Http.HttpResults;
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
            "20002",
            "1234",
            1000m);
        LoginRequest request = new()
        {
            AccountNumber = "20002",
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
        //this creates a new client 
        HttpClient client = _factory.CreateClient();

        string token =
            await LoginAsync("20003", "1234");
        //added client here after change to use bearer.
        UseBearer(client, token);


        //Act
        HttpResponseMessage response =
            await client.GetAsync(
                "api/accounts/me");
        //Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
        AccountResponse? account =
            await response.Content.ReadFromJsonAsync<AccountResponse>();
        //Had a issue with token here fixed by changing the token from 
        //private _client to client and change the bearer to create a new client for tests

        Assert.NotNull(account);
        Assert.Equal("20003", account.AccountNumber);
        Assert.Equal(1000m, account.Balance);

    }

    [Fact]
    public async Task Deposit_WithValidAmount_IncreaseBalance()
    {
        //Arrange
        await _factory.SeedAccountAsync(
            "20004",
            "1234",
            2500m);
        //Had a error here where i wrote new() instead of factory.Createclient.
        //wich cause it to make a new httpclient but not one who knows the url.
        HttpClient client = _factory.CreateClient();
        string token = await LoginAsync("20004", "1234");

        UseBearer(client, token);

        AmountRequest request = new()
        {
            Amount = 25000m
        };
        //Act
        HttpResponseMessage depositResponse =
             await client.PostAsJsonAsync(
                 "api/accounts/me/deposits",
                 request);
        //Assert
        AccountResponse? account =
            await client.GetFromJsonAsync<AccountResponse>(
                "api/accounts/me");
        Assert.NotNull(account);
        Assert.Equal(27500m, account.Balance);
        //More robust with correct status code response.
        Assert.Equal(HttpStatusCode.OK,
            depositResponse.StatusCode);
    }
    [Fact]
    public async Task Withdraw_MoreThanIsAvailable_IsRejected()
    {
        //Arrange
        await _factory.SeedAccountAsync(
            "20005",
            "1234",
            2000m);

        HttpClient client = _factory.CreateClient();

        string token = await LoginAsync("20005", "1234");

        UseBearer(client, token);

        AmountRequest request = new()
        {
            Amount = 3000m
        };
        //Act

        HttpResponseMessage response =
          await client.PostAsJsonAsync(
            "/api/accounts/me/withdrawals",
            request);

        //Assert
        AccountResponse? account =
            await client.GetFromJsonAsync<AccountResponse>(
                "api/accounts/me");
        Assert.NotNull(account);
        Assert.Equal(2000m, account.Balance);
        Assert.Equal(HttpStatusCode.BadRequest,
            response.StatusCode);

    }

    [Fact]
    public async Task Transactions_AfterDepositAndWhitdrawal_ReturnsBoth()
    {
        //Arrange
        await _factory.SeedAccountAsync(
            "20006",
            "1234",
            2000m);

        HttpClient client = _factory.CreateClient();

        string token =
            await LoginAsync("20006", "1234");

        UseBearer(client, token);

        await client.PostAsJsonAsync(
            "/api/accounts/me/withdrawals/",
        new AmountRequest { Amount = 400m });

        await client.PostAsJsonAsync(
            "/api/accounts/me/deposits/",
            new AmountRequest { Amount = 500M });

        //Act
        TransactionResponse[]? transactions =
            await client.GetFromJsonAsync<TransactionResponse[]>
            ("/api/accounts/me/transactions");

        //Assert
        Assert.NotNull(transactions);
        //Had A fail her becose i wrote it in plural not singular
        //transaction tok away s on both types and it worked.
        //Also added in BalanceAfter for a regression safety that it records
        //updatet balance.
        Assert.Contains(transactions,
            x => x.Type == "Withdrawal"
            && x.Amount == 400m
            && x.BalanceAfter == 1600m);

        Assert.Contains(transactions,
            x => x.Type == "Deposit"
            && x.Amount == 500M
            && x.BalanceAfter == 2100M);
    }

    [Fact]
    public async Task Deposits_WithZeroAmount_ReturnsBadRequest()
    {
        //Arrange
        await _factory.SeedAccountAsync(
            "20007",
            "1234",
            2000m);
        HttpClient client = _factory.CreateClient();

        string token =
           await LoginAsync("20007", "1234");
        UseBearer(client, token);

        //Act
        HttpResponseMessage response =
            await client.PostAsJsonAsync(
                "/api/accounts/me/deposits",
                new AmountRequest { Amount = 0m });
        //Assert
        Assert.Equal(
            HttpStatusCode.BadRequest,
            response.StatusCode);
    }



    [Fact]
    public async Task GetMe_TokenForAccounts_ReturnsOnlyAccountA()
    {
        //Important tests so users cant get wrong token and accses
        //Others account. if this fails shut it down;P.
        //Arrange
        await _factory.SeedAccountAsync(
            "20008",
            "1234",
            2500m);

        await _factory.SeedAccountAsync(
            "20009",
            "5678",
            1337m);

        HttpClient client = _factory.CreateClient();

        string token =
            await LoginAsync("20008", "1234");

        UseBearer(client, token);

        //Act
        AccountResponse? account =
            await client.GetFromJsonAsync<AccountResponse>(
                "/api/accounts/me");
        //Assert
        Assert.NotNull(account);
        Assert.Equal("20008", account.AccountNumber);
        Assert.Equal(2500m, account.Balance);

    }









}