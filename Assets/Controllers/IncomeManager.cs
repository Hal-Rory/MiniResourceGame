using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IncomeManager : IControllable
{
    private HashSet<IIncomeContributor> _incomeContributors = new HashSet<IIncomeContributor>();
    public int CurrentFunds { get; private set; }
    public int NetIncome { get; private set; }

    public void SetUp()
    {
        GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.RegisterListener(earlyClockUpdate:ClockUpdate);
    }

    public void SetDown()
    {
        GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.UnregisterListener(earlyClockUpdate:ClockUpdate);
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
        NetIncome = _incomeContributors.Sum(c => c.GetIncomeContribution());
        Pay(obj.GetPrice());
    }

    private void CollectPayments()
    {
        NetIncome = _incomeContributors.Sum(c => c.GetIncomeContribution());
        CurrentFunds += NetIncome;
    }

    public bool TryPurchase(int amount)
    {
        if (CurrentFunds < amount) return false;
        CurrentFunds -= amount;
        return true;
    }

    public void Pay(int amount)
    {
        CurrentFunds += amount;
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