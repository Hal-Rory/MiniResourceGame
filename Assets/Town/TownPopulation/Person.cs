using System;
using Interfaces;
using Placement;
using Town.TownObjects;
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
        public int HouseID;
        public int IncomeContribution;
        public float Happiness { get; private set; }
        public string JobName;
        public string CurrentLocation;
        public int CurrentLocationID;

        public bool CanWork => HouseholdIndex != -1 && AgeGroup != PersonAgeGroup.Deceased;

        public void Setup(string name, PersonAgeGroup ageGroup, int household, int houseID)
        {
            Name = name;
            AgeGroup = ageGroup;
            HouseholdIndex = household;
            int ageMin = (int)AgeGroup - 1; //0-1, 1-2, 2-3 ...
            int ageMax = (int)AgeGroup;
            _age = Random.Range(_ageRanges[ageMin], _ageRanges[ageMax]);
            Happiness = 0;
            HouseID = houseID;
            Employ(-1, 0, "Unemployed");
        }

        public void Employ(int placementID, int wages, string jobName)
        {
            JobIndex = placementID;
            IncomeContribution = wages;
            JobName = jobName;
        }

        public void ClockUpdate(int tick)
        {
            Happiness = 0;
            float ageFactor = tick / 100f;
            AgeUp(ageFactor);
        }

        public void SetLocation(TownLot lot)
        {
            CurrentLocation = lot.GetLotName();
            CurrentLocationID = lot.PlacementID;
            Happiness += lot.GetLotHappiness();
        }

        public void SetLocation()
        {
            CurrentLocation = "Wandering...";
            CurrentLocationID = -1;
            Happiness += 0;
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

            return AgeGroup != PersonAgeGroup.Deceased;
        }

        public void Evict()
        {
            HouseholdIndex = -1;
            CurrentLocationID = -1;
            CurrentLocation = string.Empty;
            Unemploy();
        }

        public void Unemploy()
        {
            if (CurrentLocationID == JobIndex) CurrentLocationID = -1;
            Employ(-1, 0, "Unemployed");
        }

        public string PrintJob()
        {
            return $"Job: {JobName}";
        }

        public string PrintIncome()
        {
            return $"Income: +${IncomeContribution} / d";
        }

        public string PrintLocation()
        {
            return $"Location: {CurrentLocation}";
        }

        public string PrintHappiness()
        {
            return $"Happiness: {Happiness}";
        }

        public override string ToString()
        {
            return $"{PrintJob()}" +
                   $"\n{PrintIncome()}" +
                   $"\n{PrintLocation()}" +
                   $"\n{PrintHappiness()}";
        }
    }
}