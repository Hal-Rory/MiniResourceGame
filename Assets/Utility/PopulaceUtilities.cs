using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public enum PersonAgeGroup { All, Child, Teen, Adult, Elder, Deceased }

    /// <summary>
    /// A utility for displaying several details about the age groups
    /// </summary>
    public static class PopulationUtility
    {
        public static string GroupedString(this IEnumerable<PersonAgeGroup> enumerable, bool isPlural, bool uppercase)
        {
            string ageGroup = string.Join(", ", enumerable.Select(age => isPlural ? age.Plural(uppercase).ToString() : age.ToString()).ToList());
            return ageGroup;
        }

       public static string ToNoun(this PersonAgeGroup age)
        {
            switch (age)
            {
                case PersonAgeGroup.Child:
                    return "a child";
                case PersonAgeGroup.Teen:
                    return "a teenager";
                case PersonAgeGroup.Adult:
                    return "an adult";
                case PersonAgeGroup.Elder:
                    return "an elder";
                case PersonAgeGroup.Deceased:
                default:
                    return age.ToString();
            }
        }

        public static string Plural(this PersonAgeGroup age, bool uppercase = false)
        {
            switch (age)
            {
                case PersonAgeGroup.Child:
                    return uppercase ? "Children" : "children";
                case PersonAgeGroup.Teen:
                    return uppercase ? "Teenagers" : "teenagers";
                case PersonAgeGroup.Adult:
                    return uppercase ? "Adults" : "adults";
                case PersonAgeGroup.Elder:
                    return uppercase ? "Elderly": "elderly";
                case PersonAgeGroup.Deceased:
                default:
                    return uppercase ? CapitalizeFirst(age) : age.ToString();
            }
        }

        public static string CapitalizeFirst(this PersonAgeGroup age)
        {
            return char.ToUpperInvariant(age.ToString()[0]) + age.ToString()[1..];
        }
    }
}