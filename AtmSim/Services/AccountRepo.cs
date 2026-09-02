using AtmSim.Models;
using System.Text.Json;


namespace AtmSim.Services;

internal sealed class AccountRepo
{
    private readonly string _path = Path.Combine("Data", "accounts.Json");

    public List<BankAccountData> LoadAccounts()
    {
        if (!File.Exists(_path))
            return [];
        string json = File.ReadAllText(_path);

        return JsonSerializer.Deserialize<List<BankAccountData>>(json) ?? [];

    }

    public void SavedAccounts(List<BankAccountData> accounts)
    {
        string json = JsonSerializer.Serialize(
            accounts,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(_path, json);
    }

}
