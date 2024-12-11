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
        [SerializeField] private List<Person> _inhabitants = new List<Person>();
        public int HouseID { get; private set; }
        public int HouseholdID { get; private set; }
        public string HouseholdName{ get; private set; }

        public Household(int id, int houseID, string householdName)
        {
            HouseholdID = id;
            HouseID = houseID;
            HouseholdName = householdName;
        }

        public void Set(int? id, int? houseID, string householdName)
        {
            if(id.HasValue) HouseholdID = id.Value;
            if(houseID.HasValue) HouseID = houseID.Value;
            HouseholdName = householdName;
        }

        public void AddInhabitant(Person person)
        {
            _inhabitants.Add(person);
        }

        public Person[] GetInhabitants()
        {
            return _inhabitants.ToArray();
        }

        public void Evict()
        {
            foreach (Person person in _inhabitants)
            {
                person.Evict();
            }
            _inhabitants.Clear();
        }

        public override string ToString()
        {
            string inhabitants = string.Join(", ", _inhabitants.Select(e => e.Name));

            return $"{inhabitants}";
        }
    }
}