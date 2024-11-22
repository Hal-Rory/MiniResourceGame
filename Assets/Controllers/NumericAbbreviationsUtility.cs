using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

public static class NumericAbbreviationsUtility
{
    private static readonly SortedDictionary<int, string> abbrevations = new SortedDictionary<int, string>
    {
        { 1000, "k" },
        { 1000000, "m" },
        { 1000000000, "b" }
    };

    public static string Abbreviate(this int amount, string format = "", int trailingDigitsCount = 0)
    {
        foreach ((int divisor, string abbreviation) in abbrevations.OrderByDescending(pair => pair.Key))
        {
            string trailingDigits = "";
            if(trailingDigitsCount > 0){
                int remainder = Math.Abs(amount)%divisor;
                int stringCount = remainder.ToString().Length - 1;
                trailingDigits = $".{remainder.ToString()[..Math.Min(trailingDigitsCount, stringCount)]}";
            }
            if (!(Math.Abs(amount) >= divisor)) continue;
            {
                return $"{amount / divisor}{trailingDigits}{abbreviation}";
            }
        }
        return amount.ToString(format);
    }
}