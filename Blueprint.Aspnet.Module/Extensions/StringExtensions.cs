﻿using System;
using System.Text.RegularExpressions;

namespace Blueprint.Aspnet.Module.Extensions
{
    internal static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EqualsIgnoreWhitespace(this string a, string b, StringComparison comparison)
        {
            return string.Equals(
                Regex.Replace(a, @"\s", " "),
                Regex.Replace(b, @"\s", " "),
                comparison);
        }
    }
}