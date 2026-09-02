using System.Security.Cryptography;

namespace AtmSim.Data;

internal sealed class PasswordHasher
{
    public string HashPin(string pin)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            pin,
            salt,
            100_000,//why 000 here?. and not 100 like i did last time?
            HashAlgorithmName.SHA256,
            32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    //splits the pin and hash via : and if it return a result not equal 2 parts it is not a thing(false)?.
    public bool VerifyPassword(string pin, string storedHash)
    {
        string[] parts = storedHash.Split(':');

        if (parts.Length != 2)
            return false;
        //these to arrays converts it back into json in 2 parts and gets checked above ?
        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);

        byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
            pin,
            salt,
            100_000,//again 000 at the end ?
            HashAlgorithmName.SHA256,
            32);

        return CryptographicOperations.FixedTimeEquals(
            actualHash,
            expectedHash);

    }
}
