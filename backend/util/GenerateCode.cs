namespace backend.util;

public class Code
{
    public static string GenerateBoxCode(int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random rng = new Random();

        var result = new char[length];
        for (int i = 0; i < length; i++)
        {
            int index = rng.Next(0, chars.Length);
            result[i] = chars[index];
        }

        return new string(result);
    }
}