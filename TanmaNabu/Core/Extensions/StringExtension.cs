using System;

namespace TanmaNabu.Core.Extensions
{
    public static class StringExtension
    {
        public static void Log(this string str, bool addEmptyLine = false)
        {
            Console.WriteLine(str);
            if (addEmptyLine)
            {
                Console.WriteLine();
            }
        }

        public static bool EqualsIgnoreCase(this string str, string value) => str.Equals(value, StringComparison.InvariantCultureIgnoreCase);
    }
}
