using System;
using System.Collections.Generic;
using Common.Utility;
using Controllers;
using Interfaces;
using Newtonsoft.Json;
using Town.TownObjects;
using Town.TownPopulation;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Town.TownManagement
{
    [Serializable]
    public class PopulationFactory: IControllable
    {
        [SerializeField] private List<Household> _populationHouseholds;
        [SerializeField] private List<Person> _population;
        public event Action<IPopulation> OnPopulationCreated;
        public event Action<IPopulation> OnPopulationRemoved;
        /// <summary>
        /// The available names to use for the person creation, populated by a json at start
        /// </summary>
        private Names _populationNames;
        public int PopulationCount { get; private set; }

        private class Names
        {
            public string[] first { get; set; }
            public string[] last{ get; set; }
        }

        /// <summary>
        /// Register for the lots' placement/destruction and read in a json to populate the list of person names
        /// </summary>
        public void SetUp()
        {
            _population = new List<Person>();
            _populationHouseholds = new List<Household>();
            GameController.Instance.RegisterPlacementListener(destructionListener:OnRemoveLot);
            TextAsset textFile = Resources.Load<TextAsset>("Names");
            _populationNames = JsonConvert.DeserializeObject<Names>(textFile.text);
        }

        public void SetDown()
        {
            GameController.Instance.UnregisterPlacementListener(destructionListener:OnRemoveLot);
        }

        /// <summary>
        /// When a lot is destroyed, appropriately handle any persons inside
        /// </summary>
        /// <param name="lot"></param>
        private void OnRemoveLot(TownLot lot)
        {
            switch (lot)
            {
                case House house:
                OrphanHousehold(house);
                    break;
                case not House:
                    foreach (Person person in lot.GetVisitors().ToArray())
                    {
                        RelocatePerson(person, GameController.Instance.GameTime.TimeOfDay);
                    }
                    break;
            }
        }

        /// <summary>
        /// Set up a house with a new household, place all persons within the household
        /// </summary>
        /// <param name="house"></param>
        public void CreateHome(House house)
        {
            Household household = CreateHousehold(house.MaxHouseholdCapacity, house.PlacementID);
            house.SetHousehold(household);
            PopulationCount = GetActivePopulation().Count;
            household.GetInhabitants().ForEach(GameController.Instance.GetPersonLocation);
            OnPopulationCreated?.Invoke(household);
        }

        public Household GetHousehold(int id)
        {
            return _populationHouseholds.Find(h => h.HouseID != -1 && h.HouseholdID == id);
        }

        /// <summary>
        /// Create or reuse households to fill with people
        /// </summary>
        /// <param name="size"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Household CreateHousehold(int size, int id)
        {
            int startingAdults = Random.Range(1, size);
            int startingChildren = Random.Range(0, size - startingAdults);
            string householdName = _populationNames.last.GetRandomIndex();
            Household household = _populationHouseholds.Find(h =>
            {
                if (h.HouseID != -1) return false;
                h.Set(null, id, $"{householdName} Home", size);
                return true;
            }) ?? new Household(_populationHouseholds.Count, id, $"{householdName} Home");
            if (!_populationHouseholds.Contains(household))
            {
                _populationHouseholds.Add(household);
            }
            for (int i = 0; i < startingAdults + startingChildren; i++)
            {
                Person householdMember;
                if (_population.Find(p => p.HouseholdIndex == -1) is { } person)
                {
                    householdMember = person;
                }
                else
                {
                    householdMember = new Person();
                    GameController.Instance.GameTime.RegisterListener(clockUpdate: householdMember.ClockUpdate, stateClockUpdate: timeOfDay =>
                    {
                        RelocatePerson(householdMember, timeOfDay);
                    });
                    _population.Add(householdMember);
                }
                householdMember.Setup($"{_populationNames.first.GetRandomIndex()} {householdName}",
                    i < startingAdults
                        ? (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Adult, (int)PersonAgeGroup.Elder + 1) //Adult-Elder
                        : (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Child, (int)PersonAgeGroup.Teen + 1), //Child-Teen
                    household.HouseholdID, id);
                household.AddInhabitant(householdMember);
            }
            return household;
        }

        /// <summary>
        /// Relocate a person based on the time of day
        /// </summary>
        /// <param name="person"></param>
        /// <param name="timeOfDay"></param>
        public void RelocatePerson(Person person, GameTimeManager.TimesOfDay timeOfDay)
        {
            if(person.HouseholdIndex != -1)
                GameController.Instance.GetPersonLocation(person, timeOfDay);
        }

        /// <summary>
        /// Get only the Persons in a valid household(because the households can be reused on "destruction" which keeps them in the list)
        /// </summary>
        /// <returns></returns>
        public List<Person> GetActivePopulation()
        {
            return _population.FindAll(person => person.HouseholdIndex != -1);
        }

        /// <summary>
        /// A way to shorthand _populationFactory.PopulationCount == 0 ?
        /// </summary>
        /// <returns></returns>
        public float UsePopulationAsAverage(float amount, int defaultValue = 0)
        {
            return PopulationCount == 0 ? defaultValue : amount / PopulationCount;
        }

        /// <summary>
        /// Dump the household, store it for reuse, handle/destroy the people inside
        /// </summary>
        /// <param name="house"></param>
        public void OrphanHousehold(House house)
        {
            if (house.Household == null) return;
            house.Household.Set(null, -1, null, 0);
            OnPopulationRemoved?.Invoke(house.Household);
            foreach (Person person in house.Household.GetInhabitants())
            {
                GameController.Instance.RemovePersonLocation(person);
                person.Evict();
            }
            house.Household.Evict();
            PopulationCount = GetActivePopulation().Count;
        }
    }
}