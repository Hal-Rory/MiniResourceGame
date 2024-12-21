using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

namespace Town.TownPopulation
{
    [Serializable]
    public class Household : IPopulation
    {
        private List<Person> _inhabitants = new List<Person>();
        public int HouseID { get; private set; }
        public int HouseholdID { get; private set; }
        public string HouseholdName{ get; private set; }
        public int MaxSize { get; private set; } = 1;

        public Household(int id, int houseID, string householdName)
        {
            HouseholdID = id;
            HouseID = houseID;
            HouseholdName = householdName;
        }

        public void Set(int? id, int? houseID, string householdName, int maxSize)
        {
            if(id.HasValue) HouseholdID = id.Value;
            if(houseID.HasValue) HouseID = houseID.Value;
            HouseholdName = householdName;
            MaxSize = maxSize;
        }

        public void AddInhabitant(Person person)
        {
            _inhabitants.Add(person);
        }

        public List<Person> GetInhabitants()
        {
            return _inhabitants.ToList();
        }

        public void Evict()
        {
            _inhabitants.Clear();
        }

        public override string ToString()
        {
            string inhabitants = string.Join(", ", _inhabitants.Select(e => e.Name));

            return $"{inhabitants}";
        }
    }
}