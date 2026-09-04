using System.ComponentModel.DataAnnotations;

namespace AtmApi.DTO;

public sealed class AmountRequest
{
    //limited to 1 million max per request ?
    [Range(typeof(decimal), "0,01", "1000000")]
    public decimal Amount { get; init; }
}

public sealed class AccountResponse
{
    public string AccountNumber { get; init; } = string.Empty;
    public decimal Balance { get; init; }
}

public sealed class TransactionResponse
{
    public int Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal BalanceAfter { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

