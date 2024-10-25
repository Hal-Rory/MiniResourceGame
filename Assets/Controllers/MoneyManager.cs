using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoneyManager : ITimeListener
{
    private HashSet<IIncomeContributor> _incomeContributors = new HashSet<IIncomeContributor>();
    public float CurrentIncome { get; private set; }
    public float CurrentIncomeTotal { get; private set; }
    private void OnEnable()
    {
        GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.TimeManager.RegisterListener(this, true);
    }

    private void OnDisable()
    {
        GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.TimeManager.UnregisterListener(this, true);
    }

    private void ObjectPlacerOnOnLotAdded(TownLot obj)
    {
        if (obj is IIncomeContributor contributor)
        {
            RegisterIncomeContributor(contributor);
        }
    }

    private void ObjectPlacerOnOnLotRemoved(TownLot obj)
    {
        if (obj is IIncomeContributor contributor)
        {
            UnregisterIncomeContributor(contributor);
        }
    }

    private void CollectPayments()
    {
        CurrentIncomeTotal = _incomeContributors.Sum(contributor => contributor.CanContribute ? contributor.GetIncomeContribution() : 0);
        CurrentIncome += CurrentIncomeTotal;
    }

    public void RegisterIncomeContributor(IIncomeContributor contributor)
    {
        _incomeContributors.Add(contributor);
    }

    public void UnregisterIncomeContributor(IIncomeContributor contributor)
    {
        _incomeContributors.Remove(contributor);
    }

    public void ClockUpdate(int tick)
    {
        CollectPayments();
    }
}