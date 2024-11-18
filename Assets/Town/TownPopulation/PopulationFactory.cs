using System;
using System.Collections.Generic;
using Common.Utility;
using Town.TownPopulation;
using Random = UnityEngine.Random;

[Serializable]
public class PopulationFactory: IControllable
{
    public List<Household> PopulationHouseholds { get; private set; }
    //find a way to make this a dict<int, person>.
    //Remove person, leave int, search for first instance of key->value==null and repopulate index
    //all references to the person is by index in the dict so update references on person exit
    public List<Person> Population { get; private set; }
    public event Action OnPopulationChanged;
    private List<string> _nameCollection = new List<string>
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

    public Household HomelessMatch(House house)
    {
        return PopulationHouseholds.Find(h =>
            h.Homeless && h.MemberCount <= house.HouseholdSize);
    }

    public bool HomelessMatch(House house, Household household)
    {
        return household.Homeless && household.MemberCount <= house.HouseholdSize;
    }

    public void TryCreateHome(House house, int thisHousehold)
    {
        Household household = PopulationHouseholds[thisHousehold];

        house.SetHousehold(household);
        household.SetHouseID(house.PlacementID);
        OnPopulationChanged?.Invoke();
    }

    public void TryCreateHome(House house)
    {
        Household household =
            HomelessMatch(house)
                ?? CreateHousehold(house.HouseholdSize);

        house.SetHousehold(household);
        household.SetHouseID(house.PlacementID);
        OnPopulationChanged?.Invoke();
    }

    public void CreateHome(House house)
    {
        Household household = CreateHousehold(house.HouseholdSize);
        house.SetHousehold(household);
        household.SetHouseID(house.PlacementID);
        OnPopulationChanged?.Invoke();
    }

    public Household CreateHousehold(int size)
    {
        int startingAdults = Random.Range(1, size);
        int startingChildren = Random.Range(0, size - startingAdults);
        Household household = new Household();

        for (int i = 0; i < startingAdults + startingChildren; i++)
        {
            Person householdMember = new Person
            {
                Name = $"{_nameCollection.GetRandomIndex()}",
                AgeGroup = i < startingAdults
                    ? (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Adult, (int)PersonAgeGroup.Elder + 1)
                    : (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Child, (int)PersonAgeGroup.Teen + 1),
                HouseholdIndex = PopulationHouseholds.Count,
                JobIndex = -1,
                Homeless = false
            };
            householdMember.Setup(Population.Count);
            GameController.Instance.GameTime.RegisterListener(householdMember);
            household.AddInhabitant(householdMember);
            Population.Add(householdMember);
        }

        household.FinalizeHousehold(PopulationHouseholds.Count);
        PopulationHouseholds.Add(household);
        return household;
    }

    public void OrphanHousehold(House house)
    {
        if (house.Household != null)
        {
            house.Household.SetHouseID(-1);
            foreach (Person person in house.Household.GetInhabitants())
            {
                person.Evict();
            }
        }
        OnPopulationChanged?.Invoke();
        house.SetHousehold(null);
    }
}