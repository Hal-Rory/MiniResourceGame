using System;
using UnityEngine;

namespace Common.Utility
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Checks if between two values, includes equal to
        /// </summary>
        /// <param name="thisValue"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool IsBetweenRange(this float thisValue, float value1, float value2)
        {
            return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);
        }
        
    }
    [Serializable]
    public struct RangeFloat
    {
        /// <summary>
        ///   <para>The starting index of the range, where 0 is the first position, 1 is the second, 2 is the third, and so on.</para>
        /// </summary>
        public float start;
        /// <summary>
        ///   <para>The length of the range.</para>
        /// </summary>
        public float length;

        /// <summary>
        ///   <para>The end index of the range (not inclusive).</para>
        /// </summary>
        public float end => this.start + this.length;

        /// <summary>
        ///   <para>Constructs a new RangeInt with given start, length values.</para>
        /// </summary>
        /// <param name="start">The starting index of the range.</param>
        /// <param name="length">The length of the range.</param>
        public RangeFloat(float start, float length)
        {
            this.start = start;
            this.length = length;
        }
    }
}
