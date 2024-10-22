using System;
using System.Collections.Generic;
using Common.Utility;
using Town.TownPopulation;
using UnityEngine;
using Random = UnityEngine.Random;

public class PopulationManager : MonoBehaviour
{
    [field:SerializeField] public List<Household> PopulationHouseholds { get; private set; }
    [field:SerializeField] public List<Person> Population { get; private set; }
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

    private void OnEnable()
    {
        GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
    }

    private void OnDisable()
    {
        GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
    }

    private void ObjectPlacerOnOnLotAdded(TownLot obj)
    {
        if (obj is House house)
        {
            CreateHouse(house);
            OnPopulationChanged?.Invoke();
        }
    }

    private void ObjectPlacerOnOnLotRemoved(TownLot obj)
    {
        if (obj is House house)
        {
            OrphanHousehold(house.Household);
            OnPopulationChanged?.Invoke();
        }
    }

    public void CreateHouse(House house)
    {
        Household household = CreateHousehold(house.HouseholdSize, house.PlacementID);
        house.SetHousehold(household);
    }

    public Household CreateHousehold(int size, int id)
    {
        int startingAdults = Random.Range(1, size);
        Household household = new Household();
        household.SetHouseID(id);

        Person householdMember;
        for (int i = 0; i < startingAdults; i++)
        {
            householdMember = new Person
            {
                Name = $"{_nameCollection.GetRandomIndex()}",
                AgeGroup = (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Adult, (int)PersonAgeGroup.Elder + 1),
                HouseholdIndex = PopulationHouseholds.Count,
                JobIndex = -1,
                Homeless = false
            };
            print($"{householdMember.Name} is {householdMember.AgeGroup.ToNoun()}");
            householdMember.SetAge();
            GameController.Instance.TimeManager.RegisterListener(householdMember);
            household.AddInhabitant(householdMember);
            Population.Add(householdMember);
        }
        int startingChildren = Random.Range(0, size-startingAdults);
        for (int i = 0; i < startingChildren; i++)
        {
            householdMember = new Person
            {
                Name = $"{_nameCollection.GetRandomIndex()}",
                AgeGroup = (PersonAgeGroup)Random.Range((int)PersonAgeGroup.Child, (int)PersonAgeGroup.Teen + 1),
                HouseholdIndex = PopulationHouseholds.Count,
                JobIndex = -1,
                Homeless = false
            };
            print($"{householdMember.Name} is {householdMember.AgeGroup.ToNoun()}");
            householdMember.SetAge();
            GameController.Instance.TimeManager.RegisterListener(householdMember);
            household.AddInhabitant(householdMember);
            Population.Add(householdMember);
        }

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