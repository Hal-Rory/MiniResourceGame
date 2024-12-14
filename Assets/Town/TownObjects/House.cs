using System.Collections.Generic;
using System.Linq;
using Placement;
using Town.TownPopulation;
using UnityEngine;

public class House : TownLot
{
    [field: SerializeField] public Household Household { get; private set; }
    [SerializeField] private List<Person> _inhabitants = new List<Person>();
    private HousingLotObj _houseLotObj => _townLotData as HousingLotObj;
    public int MaxHouseholdCapacity => _houseLotObj.InhabitantCapacity;

    public void SetHousehold(Household household)
    {
        Household = household;
        _inhabitants.Clear();
        _inhabitants.AddRange(Household.GetInhabitants());
        SetLotName(household?.HouseholdName);
    }

    public List<Person> GetInhabitants()
    {
        return Household?.GetInhabitants();
    }

    public int GetInhabitantsCount()
    {
        return Household != null ? Household.GetInhabitants().Count : 0;
    }

    public override string GetLotName()
    {
        return Household == null ? $"{_townLotData.Name} Home" : base.GetLotName();
    }

    public override string ToString()
    {
        string inhabitants = string.Empty;
        if (Household != null)
            inhabitants = Household.GetInhabitants().Aggregate(
                inhabitants,
                (current, inhabitant) => current + $"\n{inhabitant}");

        return $"{(Household == null ? _townLotData.Name : GetLotName())}" +
               (!string.IsNullOrEmpty(inhabitants)
                   ? $"\nResidents: {inhabitants}"
                   : "\nCurrently Vacant");
    }
}