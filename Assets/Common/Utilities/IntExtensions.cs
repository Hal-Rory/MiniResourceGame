using UnityEngine;

namespace Common.Utility
{
    public static class IntExtensions
    {    
        /// <summary>
        /// Loops the value t, so that it is never larger than length and never smaller than
        /// </summary>
        /// <param name="i"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static int Repeat(this int i, int length)
        {
            return Mathf.FloorToInt(Mathf.Repeat(i, length));
        }
        /// <summary>
        /// Checks if between two values(inclusive)
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool IsBetweenRange(this int thisValue, int value1, int value2)
        {
            return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);
        }
    }
}
