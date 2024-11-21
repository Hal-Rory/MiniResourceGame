using System.Linq;
using Placement;
using Town.TownObjectData;
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

    public int GetHousingSize()
    {
        return _houseLotData.HouseholdSize;
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