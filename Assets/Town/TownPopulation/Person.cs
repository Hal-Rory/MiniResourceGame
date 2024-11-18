using System;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
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

        public float Contentedness { get; private set; }

        public int ID { get; private set; }

        public bool CanWork => !Homeless && AgeGroup != PersonAgeGroup.Deceased;

        public void Setup(int id)
        {
            int age = (int)AgeGroup;
            int ageMin = AgeGroup == PersonAgeGroup.Child ? 1 : _ageRanges[(int)AgeGroup - 1];
            _age = Random.Range(ageMin,_ageRanges[age]);
            ID = id;
            Contentedness = .5f;
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
            float ageFactor = tick / 100f;
            if (!IsAlive(ageFactor)) return;
        }

        private bool IsAlive(float ageFactor)
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
            }

            return AgeGroup != PersonAgeGroup.Deceased;
        }

        public override string ToString()
        {
            return $"{Name} | {AgeGroup} (Income:{IncomeContribution:+0;-#})";
        }
    }
}