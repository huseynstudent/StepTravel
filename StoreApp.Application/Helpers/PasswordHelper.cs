using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreApp.Application.Helpers;

public static class PasswordHelper
{
    public static string Hash(string password)
    {
        if (password is null) password = string.Empty;
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public static bool Verify(string password, string hash)
    {
        var computed = Hash(password);
        return string.Equals(computed, hash, StringComparison.OrdinalIgnoreCase);
    }
}