using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Town.TownObjects;

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

        /// <summary>
        /// Collect payments based on the net income of all income contributors(workplaces)
        /// </summary>
        private void CollectPayments()
        {
            NetIncome = _incomeContributors.Sum(c => c.GetIncomeContribution());
            Pay(NetIncome);
        }

        /// <summary>
        /// Check if this amount can be spent and complete the transaction
        /// </summary>
        /// <param name="amount">The amount to remove(-)</param>
        /// <returns></returns>
        public bool TryPurchase(int amount)
        {
            if (!CanPurchase(amount)) return false;
            Pay(-amount);
            return true;
        }

        /// <summary>
        /// Check if this amount can be spent but do not complete the transaction
        /// </summary>
        /// <param name="amount">The amount to remove(-)</param>
        /// <returns></returns>
        public bool CanPurchase(int amount)
        {
            return CurrentFunds >= amount;
        }

        /// <summary>
        /// Complete a transaction
        /// </summary>
        /// <param name="amount">The amount to give or take(+/-)</param>
        public void Pay(int amount)
        {
            CurrentFunds += amount;
            if (amount == 0) return;
            OnIncomeUpdated?.Invoke();
            OnIncomeChanged?.Invoke(amount);
        }

        private void RegisterIncomeContributor(IIncomeContributor contributor)
        {
            _incomeContributors.Add(contributor);
        }

        private void UnregisterIncomeContributor(IIncomeContributor contributor)
        {
            _incomeContributors.Remove(contributor);
        }

        /// <summary>
        /// Payouts happen immediately after work hours
        /// </summary>
        /// <param name="timeOfDay"></param>
        private void ClockUpdate(int tick)
        {
            CollectPayments();
        }
    }
}