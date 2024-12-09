using System.Collections.Generic;
using System.Linq;
using Placement;
using Town.TownPopulation;
using UnityEngine;

public class House : TownLot
{
    [field: SerializeField] public Household Household { get; private set; }
    private HousingLotObj _houseLotData => _townLotData as HousingLotObj;

    public void SetHousehold(Household household)
    {
        Household = household;
    }

    public override List<Person> GetPersons()
    {
        return Household?.GetInhabitants().ToList();
    }

    public override int GetPersonsCount()
    {
        return Household != null ? Household.GetInhabitants().Length : 0;
    }
    public override string ToString()
    {
        string inhabitants = string.Empty;
        if (Household != null)
            inhabitants = Household.GetInhabitants().Aggregate(
                inhabitants,
                (current, inhabitant) => current + $"\n{inhabitant}");

        return $"{_townLotData.Name}" +
               (!string.IsNullOrEmpty(inhabitants)
                   ? $"\nResidents: {inhabitants}"
                   : "\nCurrently Vacant");
    }
}