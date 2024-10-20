using System;

namespace Town.TownPopulation
{
    public enum PersonAgeGroup { Child, Teen, Adult, Elder, Deceased }
    [Serializable]
    public class Person : ITimeListener
    {
        private static int[] _ageRanges => new int[]
        {
            12,
            18,
            50,
            90
        };
        public string Name;
        public PersonAgeGroup AgeGroup;
        private float _age;
        public int JobIndex = -1;
        public int HouseholdIndex;
        public int IncomeContribution;
        public bool Homeless;
        public bool CanWork => !Homeless && AgeGroup != PersonAgeGroup.Deceased;

        public void SetAge()
        {
            int age = (int)AgeGroup;
            int ageMin = AgeGroup == PersonAgeGroup.Child ? 1 : _ageRanges[(int)AgeGroup - 1];
            _age = UnityEngine.Random.Range(ageMin,_ageRanges[age]);
        }

        public void Unemploy()
        {
            JobIndex = -1;
            IncomeContribution = 0;
        }

        public void Employ(int placementID, int wages)
        {
            JobIndex = placementID;
            IncomeContribution = wages;
        }

        public void Evict()
        {
            Unemploy();
            Homeless = true;
        }

        public void ClockUpdate(int tick)
        {
            if (AgeGroup == PersonAgeGroup.Deceased) return;
            _age += tick / 100f;
            int age = (int)AgeGroup;
            if (_age > _ageRanges[age])
            {
                AgeGroup++;
            }

            if (AgeGroup == PersonAgeGroup.Deceased)
            {
                Unemploy();
            }
        }
    }
}