using Town.TownPopulation;
using UnityEngine;

public class House : TownLot, IIncomeContributor
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

    public int GetIncomeContribution()
    {
        CanContribute = Household != null;
        return IncomeContribution + Household?.GetHouseholdIncome() ?? 0;
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
        foreach (Person inhabitant in Household.GetInhabitants())
        {
            inhabitants += $"\n{inhabitant}";
        }

        return $"{_lotDescription} (Upkeep:{IncomeContribution:+0;-#})" +
               (!string.IsNullOrEmpty(inhabitants)
                   ? $"\nResidents:" +
                     $"{inhabitants}"
                   : "Currently Vacant");
    }
}