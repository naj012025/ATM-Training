using AtmApi.Models;
using AtmApi.Security;
using Microsoft.EntityFrameworkCore;

namespace AtmApi.Data;

public static class DevDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, PinHasher hasher)
    {
        if (await db.Accounts.AnyAsync())
            return;

        db.Accounts.Add(new BankAccount(
            "10001",
            hasher.HashPin("1234"),
            5000M));
        await db.SaveChangesAsync();


    }

}
