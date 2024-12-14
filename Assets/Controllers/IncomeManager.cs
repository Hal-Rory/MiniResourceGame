using System;
using System.Collections.Generic;
using System.Linq;
using Placement;
using UnityEngine;

namespace Controllers
{
    public class IncomeManager : IControllable
    {
        private HashSet<IIncomeContributor> _incomeContributors = new HashSet<IIncomeContributor>();
        public event Action OnIncomeUpdated;
        public event Action<int> OnIncomeChanged;
        private int _currentFunds;
        public int CurrentFunds{
            get => _currentFunds;
            private set => _currentFunds = Math.Clamp(value, -_currentFunds, int.MaxValue);
        }
        private int _netIncome;

        public int NetIncome
        {
            get => _netIncome;
            private set => _netIncome = Math.Clamp(value, -_netIncome, int.MaxValue);
        }

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
            Pay(obj.GetLotPrice());
        }

        private void CollectPayments()
        {
            NetIncome = _incomeContributors.Sum(c => c.GetIncomeContribution());
            Pay(NetIncome);
        }

        public bool TryPurchase(int amount)
        {
            if (!CanPurchase(amount)) return false;
            Pay(-amount);
            return true;
        }

        public bool CanPurchase(int amount)
        {
            return CurrentFunds >= amount;
        }

        public void Pay(int amount)
        {
            CurrentFunds += amount;
            if (amount == 0) return;
            OnIncomeUpdated?.Invoke();
            OnIncomeChanged?.Invoke((int)Mathf.Sign(amount));
        }

        private void RegisterIncomeContributor(IIncomeContributor contributor)
        {
            _incomeContributors.Add(contributor);
        }

        private void UnregisterIncomeContributor(IIncomeContributor contributor)
        {
            _incomeContributors.Remove(contributor);
        }

        private void ClockUpdate(int tick)
        {
            CollectPayments();
        }
    }
}