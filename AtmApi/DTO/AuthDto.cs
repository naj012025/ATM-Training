namespace AtmApi.DTO;

public sealed class LoginRequest
{
    public string AccountNumber { get; init; } = string.Empty;
    public string Pin { get; init; } = string.Empty;
}

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
}

