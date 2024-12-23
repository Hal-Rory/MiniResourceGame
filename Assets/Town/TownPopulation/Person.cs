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
        //Determines the age of each age group
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

        /// <summary>
        /// Checks if a person is not homeless or dead
        /// </summary>
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

        /// <summary>
        /// On clock update, age
        /// </summary>
        /// <param name="tick"></param>
        public void ClockUpdate(int tick)
        {
            Happiness = 0;
            float ageFactor = tick / 100f;
            AgeUp(ageFactor);
        }

        /// <summary>
        /// Set the location based on a lot
        /// </summary>
        /// <param name="lot"></param>
        public void SetLocation(TownLot lot)
        {
            CurrentLocation = lot.GetLotName();
            CurrentLocationID = lot.PlacementID;
            Happiness += lot.GetLotHappiness();
        }

        /// <summary>
        /// Set the location to empty if a location is not found
        /// </summary>
        public void SetLocation()
        {
            CurrentLocation = "Wandering...";
            CurrentLocationID = -1;
            Happiness += 0;
        }

        /// <summary>
        /// Check with age range a person is in and update the age group
        /// </summary>
        /// <param name="ageFactor"></param>
        /// <returns></returns>
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

        #region Helpers
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
        #endregion

        public override string ToString()
        {
            return $"{PrintJob()}" +
                   $"\n{PrintIncome()}" +
                   $"\n{PrintLocation()}" +
                   $"\n{PrintHappiness()}";
        }
    }
}