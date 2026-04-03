namespace StoreApp.Application.Helpers
{
    public static class PasswordGenerator
    {
        private static readonly string Lower = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly string Digits = "0123456789";
        private static readonly string Specials = "@$!%*?&";
        private static readonly Random Rng = new();

        public static string Generate(int length = 8)
        {
            if (length < 6) length = 6;
            var chars = Lower + Upper + Digits + Specials;
            string password;
            do
            {
                password = new string(Enumerable.Repeat(chars, length)
                    .Select(s => s[Rng.Next(s.Length)]).ToArray());
            }
            while (!IsValid(password));
            return password;
        }

        private static bool IsValid(string password)
        {
            return password.Any(char.IsLower)
                && password.Any(char.IsUpper)
                && password.Any(char.IsDigit)
                && password.Any(c => Specials.Contains(c));
        }
    }
}

