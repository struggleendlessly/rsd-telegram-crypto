namespace eth_shared.Extensions
{
    public static class StringExt
    {
        public static string FormatTo18(this string input, string decimalCeparator)
        {
            if (input.Length < 18)
            {
                input = input.PadLeft(18, '0');
            }
            var res = input.Insert(input.Length - 18, decimalCeparator);

            return res;
        }

        public static string GetFirstThreeAfterComma(this string input, string decimalCeparator)
        {
            int commaIndex = input.IndexOf(decimalCeparator);

            if (commaIndex != -1 && commaIndex + 4 <= input.Length)
            {
                var res = input.Substring(0, commaIndex + 4);

                return res;
            }

            return input; // Return the original string if comma not found or not enough characters after comma
        }
        public static string InsertComma(
            this string input,
            int indexFromEnd,
            string decimalCeparator)
        {
            if (input.Length < indexFromEnd)
            {
                return input;
            }

            int index = input.Length - indexFromEnd;

            var res = input.Insert(index, decimalCeparator);

            return res;
        }
    }
}
