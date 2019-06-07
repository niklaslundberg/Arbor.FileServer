using System;

namespace Arbor.FileServer.Tests.Unit
{
    internal static class StringExtensions
    {
        public static string EnsureEndsWith(this string value, string ending, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
            }

            if (string.IsNullOrWhiteSpace(ending))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(ending));
            }

            if (value.EndsWith(ending, stringComparison))
            {
                return value;
            }

            return value + ending;
        }
    }
}
