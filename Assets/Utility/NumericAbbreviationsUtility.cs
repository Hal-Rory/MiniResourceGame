using System;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public static class NumericAbbreviationsUtility
    {
        private static readonly SortedDictionary<int, string> _abbrevations = new SortedDictionary<int, string>
        {
            { 1000, "k" },
            { 1000000, "m" },
            { 1000000000, "b" }
        };

        public static string Abbreviate(this int amount, int abbreviateTo, string format = "", int trailingDigitsCount = 0)
        {
            int current = 0;
            if (amount == 0) return amount.ToString(format);
            foreach ((int divisor, string abbreviation) in _abbrevations.OrderByDescending(pair => pair.Key))
            {
                if (current > abbreviateTo) break;
                current++;
                string trailingDigits = "";
                if(trailingDigitsCount > 0){
                    int remainder = Math.Abs(amount)%divisor;
                    int stringCount = remainder.ToString().Length - 1;
                    trailingDigits = $".{remainder.ToString()[..Math.Min(trailingDigitsCount, stringCount)]}";
                }
                if (!(Math.Abs(amount) >= divisor)) continue;
                return $"{amount / divisor}{trailingDigits}{abbreviation}";
            }
            return amount.ToString(format);
        }
    }
}