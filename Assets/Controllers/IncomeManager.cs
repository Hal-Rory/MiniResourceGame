using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IncomeManager : ITimeListener, IControllable
{
    private HashSet<IIncomeContributor> _incomeContributors = new HashSet<IIncomeContributor>();
    public int CurrentFunds { get; private set; }
    public int NetIncome { get; private set; }

    public void SetUp()
    {
        GameController.Instance.RegisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.RegisterListener(this, true);
    }

    public void SetDown()
    {
        GameController.Instance.UnregisterPlacementListener(ObjectPlacerOnOnLotAdded, ObjectPlacerOnOnLotRemoved);
        GameController.Instance.GameTime.UnregisterListener(this, true);
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
        NetIncome = _incomeContributors.Sum(contributor => contributor.GetIncomeContribution());
        CurrentFunds += NetIncome;
    }

    public bool TryPurchase(int amount)
    {
        if (CurrentFunds < amount) return false;
        CurrentFunds -= amount;
        return true;
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