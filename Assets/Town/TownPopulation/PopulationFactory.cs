using System;
using System.Collections.Generic;
using Common.Utility;
using Town.TownPopulation;
using Random = UnityEngine.Random;

[Serializable]
public class PopulationFactory
{
    public List<Household> PopulationHouseholds { get; private set; }
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

    public void Start()
    {
        Population = new List<Person>();
        PopulationHouseholds = new List<Household>();
        GameController.Instance.RegisterPlacementListener(OnNewLot, OnRemoveLot);
    }

    private void OnNewLot(TownLot obj)
    {
        if (obj is not House house) return;
        CreateHouse(house);
        OnPopulationChanged?.Invoke();
    }

    private void OnRemoveLot(TownLot obj)
    {
        if (obj is not House house) return;
        OrphanHousehold(house.Household);
        OnPopulationChanged?.Invoke();
    }

    public void CreateHouse(House house)
    {
        Household household = CreateHousehold(house.HouseholdSize, house.PlacementID);
        house.SetHousehold(household);
    }

    public Household CreateHousehold(int size, int id)
    {
        int startingAdults = Random.Range(1, size);
        int startingChildren = Random.Range(0, size - startingAdults);
        Household household = new Household();
        household.SetHouseID(id);

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
            GameController.Instance.TimeManager.RegisterListener(householdMember);
            household.AddInhabitant(householdMember);
            Population.Add(householdMember);
        }

        household.FinalizeHousehold();
        PopulationHouseholds.Add(household);
        return household;
    }

    public void OrphanHousehold(Household household)
    {
        household.SetHouseID(-1);
        foreach (Person person in household.GetInhabitants())
        {
            person.Evict();
        }
    }
}