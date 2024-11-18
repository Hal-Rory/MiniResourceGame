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

        public Household(int id, int houseID)
        {
            HouseholdID = id;
            HouseID = houseID;
        }

        public void AddInhabitant(Person person)
        {
            _inhabitants.Add(person);
        }

        public Person[] GetInhabitants()
        {
            return _inhabitants.ToArray();
        }

        public void ClearHousehold()
        {
            HouseholdID = -1;
        }

        public override string ToString()
        {
            string inhabitants = string.Join(", ", _inhabitants.Select(e => e.Name));

            return $"{inhabitants}";
        }
    }
}