using System;
using System.Linq;

namespace TanmaNabu.Core.Extensions
{
    public static class EnumExtension
    {
        public static string AllowedValues<T>(this T item)
        {
            return $"Allowed values: {string.Join(", ", Enum.GetValues(item.GetType()).Cast<T>().ToList())}";
        }
    }
}
