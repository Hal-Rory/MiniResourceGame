using System;
using Interfaces;
using Placement;
using Utility;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
    [Serializable]
    public class Person : IPopulation
    {
        private static int[] _ageRanges => new int[]
        {
            0,
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
        public event Action<Person> LifeCycleEnded;
        public float Happiness { get; private set; }
        public string CurrentLocation;

        public bool CanWork => HouseholdIndex != -1 && AgeGroup != PersonAgeGroup.Deceased;

        public void Setup(string name, PersonAgeGroup ageGroup, int jobIndex, int household)
        {
            Name = name;
            AgeGroup = ageGroup;
            JobIndex = jobIndex;
            HouseholdIndex = household;
            int ageMin = (int)AgeGroup - 1; //0-1, 1-2, 2-3 ...
            int ageMax = (int)AgeGroup;
            _age = Random.Range(_ageRanges[ageMin], _ageRanges[ageMax]);
            Happiness = 1f;
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

        public void ClockUpdate(int tick)
        {
            float ageFactor = tick / 100f;
            AgeUp(ageFactor);
        }

        public void SetLocation(TownLot lot)
        {
            CurrentLocation = lot.GetName();
            Happiness += lot.GetHappiness();
        }

        private bool AgeUp(float ageFactor)
        {
            if (AgeGroup == PersonAgeGroup.Deceased) return false;

            _age += ageFactor;
            int age = (int)AgeGroup;
            if (_age > _ageRanges[age])
            {
                AgeGroup++;
            }

            if (AgeGroup == PersonAgeGroup.Deceased)
            {
                Unemploy();
                LifeCycleEnded?.Invoke(this);
                LifeCycleEnded = null;
            }

            return AgeGroup != PersonAgeGroup.Deceased;
        }

        public void Evict()
        {
            HouseholdIndex = -1;
        }

    public override string ToString()
        {
            return $"Age: {AgeGroup}\n" +
                   $"Income: {IncomeContribution:+0;-#})\n" +
                   $"Location: {CurrentLocation}";
        }
    }
}