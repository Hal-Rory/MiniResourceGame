using System;
using Interfaces;
using Utility;
using static GameTimeManager;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
    [Serializable]
    public class Person : IPopulation
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
        public event Action<Person> LifeCycleEnded;
        public float Contentedness { get; private set; }
        public TownLotObj CurrentLocation;
        public int ID { get; private set; }

        public bool CanWork => HouseholdIndex != -1 && AgeGroup != PersonAgeGroup.Deceased;

        public void Setup(string name, PersonAgeGroup ageGroup, int jobIndex, int householdIndex)
        {
            Name = name;
            AgeGroup = ageGroup;
            JobIndex = jobIndex;
            HouseholdIndex = householdIndex;
            int age = (int)AgeGroup;
            int ageMin = AgeGroup == PersonAgeGroup.Child ? 1 : _ageRanges[(int)AgeGroup - 1];
            _age = Random.Range(ageMin,_ageRanges[age]);
            Contentedness = .5f;
        }

        public void Setup(int id)
        {
            ID = id;
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

        public void StateClockUpdate(TimesOfDay timeOfDay)
        {

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

        public override string ToString()
        {
            return $"{Name} | {AgeGroup} (Income:{IncomeContribution:+0;-#})";
        }
    }
}