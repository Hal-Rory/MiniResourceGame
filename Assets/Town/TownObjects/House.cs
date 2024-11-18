using System.Linq;
using Town.TownPopulation;
using UnityEngine;

public class House : TownLot
{
    [field: SerializeField] public int IncomeContribution { get; private set; }

    [field: SerializeField] public bool CanContribute { get; private set; }

    [field: SerializeField] public int HouseholdSize { get; private set; }

    [field: SerializeField] public Household Household { get; private set; }

    [SerializeField] private GameObject _hoverBG;

    public void SetHousehold(Household household)
    {
        Household = household;
    }

    public override void StartHovering()
    {
        _hoverBG.SetActive(true);
    }

    public override void EndHovering()
    {
        _hoverBG.SetActive(false);
    }

    public override void Create(TownLotObj lotObj)
    {
        _lotDescription = lotObj.Name;
        _lotDepiction = lotObj.ObjPreview;
    }

    public override string ToString()
    {
        string inhabitants = string.Empty;
        if (Household != null)
            inhabitants = Household.GetInhabitants().Aggregate(
                inhabitants,
                (current, inhabitant) => current + $"\n{inhabitant}");


        // return $"{_lotDescription} (Upkeep: {GetIncomeContribution():0;-#})" +
        //        (!string.IsNullOrEmpty(inhabitants)
        //            ? $"\nResidents: {inhabitants}"
        //            : "\nCurrently Vacant");

        return $"{_lotDescription}" +
               (!string.IsNullOrEmpty(inhabitants)
                   ? $"\nResidents: {inhabitants}"
                   : "\nCurrently Vacant");
    }
}