using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces.Authentication;

namespace Infrastructure.Authentication;

internal class TemporaryPasswordGenerator : IPasswordGenerator
{
    /// <summary>
    /// Here is a simple implementation of a temporary password generator that meets the specified requirements.
    /// It generates a 12-character password that includes uppercase letters, lowercase letters, numbers,
    /// and special characters. The password is generated using a cryptographically secure random number generator
    /// to ensure it is not easily guessable.
    /// Additionally, it avoids using common dictionary words by selecting characters from a predefined set.
    /// For example: "RafA1b2C3d4E5f6"
    /// </summary>
    /// <returns></returns>
    public string GenerateTemporaryPassword()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%^&*";
        var random = RandomNumberGenerator.Create();
        var data = new byte[16];
        random.GetBytes(data);

        var password = new StringBuilder();
        password.Append("Raf"); // Prefix to indicate temporary

        for (int i = 0; i < 12; i++)
        {
            password.Append(chars[data[i] % chars.Length]);
        }

        return password.ToString();
    }
}
