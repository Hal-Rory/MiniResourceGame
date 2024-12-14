using System.Linq;
using Placement;
using Town.TownPopulation;
using UnityEngine;

public class House : TownLot
{
    [field: SerializeField] public Household Household { get; private set; }

    public void SetHousehold(Household household)
    {
        Household = household;
        _persons.Clear();
        _persons.AddRange(Household.GetInhabitants());
        SetName(household?.HouseholdName);
    }

    public override string GetName()
    {
        return Household == null ? $"{_townLotData.Name} Home" : base.GetName();
    }

    public override string ToString()
    {
        string inhabitants = string.Empty;
        if (Household != null)
            inhabitants = Household.GetInhabitants().Aggregate(
                inhabitants,
                (current, inhabitant) => current + $"\n{inhabitant}");

        return $"{(Household == null ? _townLotData.Name : GetName())}" +
               (!string.IsNullOrEmpty(inhabitants)
                   ? $"\nResidents: {inhabitants}"
                   : "\nCurrently Vacant");
    }
}