using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Controllers;
using Town.TownObjects;
using Random = UnityEngine.Random;

namespace Town.TownManagement
{
    [Serializable]
    public class TownPopulaceManager : IControllable
    {
        private int _totalHappiness;
        private int _totalPopulation;
        private int _growthThreshold;
        private List<int> _availableHousing;
        public int LastHappinessUpdate { get; private set; }

        private PopulationFactory _populationFactory => GameController.Instance.Population;
        private IncomeManager _incomeManager => GameController.Instance.Income;

        public void SetUp()
        {
            GameController.Instance.GameTime.RegisterListener(earlyClockUpdate: ClockUpdate);
            GameController.Instance.RegisterPlacementListener(OnNewLot, OnRemoveLot);
            _availableHousing = new List<int>();
            _totalHappiness = 0;
        }

        public void SetDown()
        {
            GameController.Instance.GameTime.UnregisterListener(earlyClockUpdate: ClockUpdate);
            GameController.Instance.UnregisterPlacementListener(OnNewLot, OnRemoveLot);
        }

        /// <summary>
        /// Checks if the population can be supported by getting the average net income of the current population
        /// as well as the average happiness of the population. Both have to reach a certain threshold
        /// </summary>
        /// <returns></returns>
        public bool CanPopulationGrow()
        {
            //is the net income able to support the population count
            _growthThreshold = (int)_populationFactory.UsePopulationAsAverage(_incomeManager.NetIncome, 1);
            //is the average high enough to warrant new growth?
            float happinessValue = Random.value;
            float averageHappiness =
                _populationFactory.UsePopulationAsAverage(_totalHappiness, 1);
            return _growthThreshold >= _populationFactory.GetActivePopulation().Count && happinessValue <= averageHappiness;
        }

        public int GetHappiness()
        {
            return _totalHappiness;
        }

        /// <summary>
        /// When a housing lot is created, add it to the list of available housing
        /// </summary>
        /// <param name="obj"></param>
        private void OnNewLot(TownLot obj)
        {
            if (obj is not House) return;
            _availableHousing.Add(obj.PlacementID);
        }

        /// <summary>
        /// When a housing lot is destroyed, remove it from the list of available housing if it was previously available
        /// </summary>
        /// <param name="obj"></param>
        private void OnRemoveLot(TownLot obj)
        {
            if (obj is not House) return;
            _availableHousing.Remove(obj.PlacementID);
        }

        /// <summary>
        /// On early clock update, adjust the current happiness total from the previous day and check if new residents can move in
        /// </summary>
        /// <param name="tick"></param>
        public void ClockUpdate(int tick)
        {
            AdjustStockpiles();
            CheckHouseholdAvailability();
        }

        private void AdjustStockpiles()
        {
            //average happiness of people
            LastHappinessUpdate = (int)_populationFactory.UsePopulationAsAverage(_populationFactory.GetActivePopulation()
                .Sum(person => person.Happiness));
            _totalHappiness += LastHappinessUpdate;
        }

        /// <summary>
        /// Attempts to move in a new household
        /// </summary>
        private void CheckHouseholdAvailability()
        {
            if (!CanPopulationGrow() || _availableHousing.Count == 0) return;
            int nextAvailable = _availableHousing.GetRandomIndex();
            House house = GameController.Instance.LotFactory.GetLot(nextAvailable) as House;
            _availableHousing.Remove(nextAvailable);
            GameController.Instance.Population.CreateHome(house);
        }
    }
}