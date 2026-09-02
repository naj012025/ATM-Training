using AtmSim.Data;
using AtmSim.Models;
using AtmSim.Services;
using System.Text;


PasswordHasher hasher = new();
AccountRepo repository = new();

List<BankAccountData> accounts = repository.LoadAccounts();


Console.WriteLine("Account Number: ?");
string? accountNumber = Console.ReadLine();

BankAccountData? account = accounts.FirstOrDefault(
    a => a.AccountNumber == accountNumber);

if (account is null)
{
    Console.WriteLine("Account not Found!");
    return;
}

int attempts = 0;
bool isAuthenticated = false;

while (attempts < 3)
{
    Console.Write("Enter PIN: ");
    string enteredPin = ReadPin();

    if (hasher.VerifyPassword(enteredPin, account.PinHash))
    {
        isAuthenticated = true;
        Console.WriteLine("Login successful.\n");
        break;
    }

    attempts++;
    Console.WriteLine($"Incorrect PIN." +
        $" Attempts left: {3 - attempts}");
}

if (!isAuthenticated)
{
    Console.WriteLine("Too many incorrect attempts. " +
        "Assassins has been dispatched Good Luck!");
    return;
}

while (true)
{

    Console.WriteLine("1. Check Balance.");
    Console.WriteLine("2. Deposit.");
    Console.WriteLine("3. Withdraw.");
    Console.WriteLine("4. Exit.");

    string? choise = Console.ReadLine();

    switch (choise)
    {
        case "1":
            Console.Clear();
            Console.WriteLine($"The Balance is: {account.Balance}\n");
            break;

        case "2":
            Console.WriteLine("How much would you like to deposit?.");
            if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
            {
                account.Deposit(depositAmount);
                Console.Clear();
                Console.WriteLine($"New Balance: {account.Balance}\n");
            }
            else
            {
                Console.WriteLine("Invalid Amount");
            }
            break;

        case "3":
            Console.WriteLine("How much would you like to withdraw?.");
            if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
            {
                account.Withdraw(withdrawAmount);
                Console.Clear();
                Console.WriteLine($"new Balance:\n{account.Balance}\n");
            }
            else
            {
                Console.WriteLine("Invalid Amount");
            }
            break;
        case "4":

            account.Balance = account.Balance;
            repository.SavedAccounts(accounts);
            Console.Clear();
            Console.WriteLine("Have a nice day!");
            return;
        default:
            Console.WriteLine("Invalid choise!.");
            break;
    }

}

static string ReadPin()
{
    StringBuilder pin = new();

    while (true)
    {
        ConsoleKeyInfo key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            return pin.ToString();
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (pin.Length > 0)
            {
                pin.Remove(pin.Length - 1, 1);
                Console.Write("\b \b");
            }
            continue;
        }
        if (char.IsDigit(key.KeyChar))
        {
            pin.Append(key.KeyChar);
            Console.Write("*");
        }
    }
}
