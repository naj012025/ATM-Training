using AtmApi.Models;
using Microsoft.EntityFrameworkCore;


namespace AtmApi.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<BankAccount> Accounts => Set<BankAccount>();
    public DbSet<BankTransaction> Transactions => Set<BankTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AccountNumber).IsUnique();
            entity.Property(x => x.AccountNumber).HasMaxLength(32).IsRequired();
            entity.Property(x => x.PinHash).HasMaxLength(512).IsRequired();
            entity.Property(x => x.Balance).HasPrecision(18, 2);//Gives max numbers 18 and followd by 2 decimals in postgres.
        });

        modelBuilder.Entity<BankTransaction>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Amount).HasPrecision(18, 2);
            entity.Property(x => x.BalanceAfter).HasPrecision(18, 2);
            entity.Property(x => x.Type).HasConversion<string>(); //converts the type into a string in postgres.

        });
    }
}



