using Town.TownPopulation;
using UnityEngine;

public class House : TownLot, IIncomeContributor
{
    [field: SerializeField] public int IncomeContribution { get; private set; }

    [field: SerializeField] public bool CanContribute { get; private set; }

    [field: SerializeField] public int HouseholdSize { get; private set; }

    [field: SerializeField] public Household Household { get; private set; }

    public void SetHousehold(Household household)
    {
        Household = household;
    }

    public int GetIncomeContribution()
    {
        CanContribute = Household != null;
        return IncomeContribution + Household?.GetHouseholdIncome() ?? 0;
    }

    public override void Create(TownObj obj)
    {
        _lotDescription = obj.Name;
        _lotDepiction = obj.ObjPreview;
    }

    public override string ToString()
    {
        string inhabitants = string.Empty;
        foreach (Person inhabitant in Household.GetInhabitants())
        {
            inhabitants += $"\n{inhabitant.Name}(Income:{inhabitant.IncomeContribution:+0;-#})";
        }

        return $"{_lotDescription} (Upkeep:{IncomeContribution:+0;-#})" +
               (!string.IsNullOrEmpty(inhabitants)
                   ? $"\nResidents:" +
                     $"{inhabitants}"
                   : "Currently Vacant");
    }
}