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
    }
}