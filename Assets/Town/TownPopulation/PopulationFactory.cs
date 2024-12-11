using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Controllers;
using Interfaces;
using Newtonsoft.Json;
using Placement;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
    [Serializable]
    public class PopulationFactory: IControllable
    {
        //find a way to make this a dict<int, person>.
        //Remove person, leave int, use .Select(first instance of key->value==null) and repopulate index
        //all references to the person is by index in the dict so update references on person exit
        public List<Household> PopulationHouseholds { get; private set; }
        public List<Person> Population { get; private set; }
        public event Action<IPopulation> OnPopulationChanged;
        private Names _populationNames;

        private class Names
        {
            public string[] first { get; set; }
            public string[] last{ get; set; }
        }

        public void SetUp()
        {
            Population = new List<Person>();
            PopulationHouseholds = new List<Household>();
            GameController.Instance.RegisterPlacementListener(destructionListener:OnRemoveLot);
            TextAsset textFile = Resources.Load<TextAsset>("Names");
            _populationNames = JsonConvert.DeserializeObject<Names>(textFile.text);
        }

        public void SetDown()
        {
            GameController.Instance.UnregisterPlacementListener(destructionListener:OnRemoveLot);
        }
        private void OnRemoveLot(TownLot obj)
        {
            if (obj is not House house) return;
            OrphanHousehold(house);
        }

        public void CreateHome(House house)
        {
            Household household = CreateHousehold(house.GetMaxCapacity(), house.PlacementID);
            house.SetHousehold(household);
            OnPopulationChanged?.Invoke(household);
        }

        public Household GetHousehold(int id)
        {
            return PopulationHouseholds.Find(h => h.HouseID != -1 && h.HouseholdID == id);
        }

        public Household CreateHousehold(int size, int id)
        {
            int startingAdults = Random.Range(1, size);
            int startingChildren = Random.Range(0, size - startingAdults);
            string householdName = $"{_populationNames.last.GetRandomIndex()} Home";
            Household household = PopulationHouseholds.Find(h =>
            {
                if (h.HouseID != -1) return false;
                h.Set(null, id,householdName);
                return true;
            }) ?? new Household(PopulationHouseholds.Count, id, householdName);
            if (!PopulationHouseholds.Contains(household))
            {
                PopulationHouseholds.Add(household);
            }
            for (int i = 0; i < startingAdults + startingChildren; i++)
            {
                Person householdMember;
                if (Population.Find(p => p.HouseholdIndex == -1) is { } person)
                {
                    householdMember = person;
                }
                else
                {
                    householdMember = new Person();
                    householdMember.LifeCycleEnded += OnPopulationChanged;
                    GameController.Instance.GameTime.RegisterListener(clockUpdate: householdMember.ClockUpdate, stateClockUpdate: timeOfDay =>
                    {
                        if(householdMember.HouseholdIndex != -1)
                            GameController.Instance.GetPersonLocation(householdMember, timeOfDay);
                    });
                    Population.Add(householdMember);
                }
                householdMember.Setup($"{_populationNames.first.GetRandomIndex()} {householdName}",
                    i < startingAdults
                        ? (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Adult, (int)PersonAgeGroup.Elder + 1) //Adult-Elder
                        : (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Child, (int)PersonAgeGroup.Teen + 1), //Child-Teen
                    -1,household.HouseholdID);
                household.AddInhabitant(householdMember);
                GameController.Instance.GetPersonLocation(householdMember);
            }
            return household;
        }

        public int GetActivePopulationCountString()
        {
            return Population.Count(p => p.HouseholdIndex != -1);
        }

        public void OrphanHousehold(House house)
        {
            if (house.Household != null)
            {
                house.Household.Set(null, -1, null);
                house.Household.Evict();
                OnPopulationChanged?.Invoke(house.Household);
                house.SetHousehold(null);
            }
        }
    }
}