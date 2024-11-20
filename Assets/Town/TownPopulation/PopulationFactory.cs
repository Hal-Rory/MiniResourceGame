using System;
using System.Collections.Generic;
using Common.Utility;
using Interfaces;
using Utility;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
    [Serializable]
    public class PopulationFactory: IControllable
    {
        public List<Household> PopulationHouseholds { get; private set; }
        //find a way to make this a dict<int, person>.
        //Remove person, leave int, search for first instance of key->value==null and repopulate index
        //all references to the person is by index in the dict so update references on person exit
        public List<Person> Population { get; private set; }
        private Stack<Person> _orphanedPersons;
        public event Action<IPopulation> OnPopulationChanged;
        public static readonly List<string> NameCollection = new List<string>
        {
            "Alice",
            "Bob",
            "Charlie",
            "Diana",
            "Ethan",
            "Fiona",
            "George",
            "Hannah",
            "Isaac",
            "Julia",
            "Kevin",
            "Lily",
            "Michael",
            "Nina",
            "Oscar"
        };

        public void SetUp()
        {
            Population = new List<Person>();
            PopulationHouseholds = new List<Household>();
            _orphanedPersons = new Stack<Person>();
            GameController.Instance.RegisterPlacementListener(destructionListener:OnRemoveLot);
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
            Household household = CreateHousehold(house.GetHousingSize(), house.PlacementID);
            house.SetHousehold(household);
            OnPopulationChanged?.Invoke(household);
        }

        public Household CreateHousehold(int size, int id)
        {
            int startingAdults = Random.Range(1, size);
            int startingChildren = Random.Range(0, size - startingAdults);
            Household household = new Household(PopulationHouseholds.Count, id);
            PopulationHouseholds.Add(household);
            for (int i = 0; i < startingAdults + startingChildren; i++)
            {
                Person householdMember;
                if (_orphanedPersons.Count > 0)
                {
                    householdMember = _orphanedPersons.Pop();
                }
                else
                {
                    householdMember = new Person();
                    householdMember.LifeCycleEnded += OnPopulationChanged;
                    householdMember.Setup(Population.Count);
                    GameController.Instance.GameTime.RegisterListener(clockUpdate: householdMember.ClockUpdate, stateClockUpdate: timeOfDay =>
                    {
                        if(householdMember.HouseholdIndex != -1 && householdMember.HouseholdIndex < PopulationHouseholds.Count)
                            GameController.Instance.GetPersonLocation(householdMember, timeOfDay);
                    });
                }
                householdMember.Setup($"{NameCollection.GetRandomIndex()}",
                    i < startingAdults
                        ? (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Adult, (int)PersonAgeGroup.Elder + 1) //Adult-Elder
                        : (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Child, (int)PersonAgeGroup.Teen + 1), //Child-Teen
                    -1,household.HouseholdID);
                household.AddInhabitant(householdMember);
                Population.Add(householdMember);
                GameController.Instance.GetPersonLocation(householdMember);
            }
            return household;
        }

        public void GetPersonLocation()
        {
        }

        public void OrphanHousehold(House house)
        {
            if (house.Household != null)
            {
                foreach (Person person in house.Household.GetInhabitants())
                {
                    person.HouseholdIndex = -1;
                    Population.Remove(person);
                    _orphanedPersons.Push(person);
                }
            }
            OnPopulationChanged?.Invoke(house.Household);
            PopulationHouseholds.Remove(house.Household);
            house.SetHousehold(null);
        }
    }
}