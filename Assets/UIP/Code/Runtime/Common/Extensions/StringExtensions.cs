namespace UIP.Runtime.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToSpacedUpperCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            System.Text.StringBuilder result = new System.Text.StringBuilder();

            foreach (char c in input)
            {
                if (char.IsUpper(c) && result.Length > 0)
                {
                    result.Append(' ');
                }
                result.Append(char.ToUpper(c));
            }

            return result.ToString();
        }
    }
}