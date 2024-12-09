using System.Collections.Generic;
using System.Linq;

namespace Utility
{
    public enum PersonAgeGroup { None, Child, Teen, Adult, Elder, Deceased, All }

    public static class PopulationUtility
    {
        public static string GroupedString(this IEnumerable<PersonAgeGroup> enumerable, bool isPlural)
        {
            string ageGroup = string.Join(", ", enumerable.Select(age => isPlural ? age.Plural().ToString() : age.ToString()).ToList());
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

        public static string Plural(this PersonAgeGroup age)
        {
            switch (age)
            {
                case PersonAgeGroup.Child:
                    return "children";
                case PersonAgeGroup.Teen:
                    return "teenagers";
                case PersonAgeGroup.Adult:
                    return "adults";
                case PersonAgeGroup.Elder:
                    return "elderly";
                case PersonAgeGroup.Deceased:
                default:
                    return age.ToString();
            }
        }
    }
}