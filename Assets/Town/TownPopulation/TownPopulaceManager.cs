using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Controllers;
using Placement;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
    [Serializable]
    public class TownPopulaceManager : IControllable
    {
        private float _totalHappiness;
        private int _totalPopulation;
        private int _growthThreshold;
        private List<int> _availableHousing;

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

        public bool CanPopulationGrow()
        {
            //is the net income able to support the population count
            _growthThreshold = (int)_populationFactory.UsePopulationAsAverage(_incomeManager.NetIncome, 1);
            //is the average high enough to warrant new growth?
            float happinessValue = Random.value;
            float averageHappiness =
                _populationFactory.UsePopulationAsAverage(_totalHappiness / 100, 1);
            return _growthThreshold >= _populationFactory.GetActivePopulation().Count && happinessValue <= averageHappiness;
        }

        public int GetHappiness()
        {
            return (int)_totalHappiness;
        }

        private void OnNewLot(TownLot obj)
        {
            if (obj is not House) return;
            _availableHousing.Add(obj.PlacementID);
        }

        private void OnRemoveLot(TownLot obj)
        {
            if (obj is not House) return;
            _availableHousing.Remove(obj.PlacementID);
        }

        public void ClockUpdate(int tick)
        {
            AdjustStockpiles();
            CheckHouseholdAvailability();
        }

        private void AdjustStockpiles()
        {
            //how crowded are the households
            float densityHappinessDecay = _populationFactory.AverageHouseholdSize();
            //average happiness of people
            float averageHappiness = _populationFactory.UsePopulationAsAverage(_populationFactory.GetActivePopulation()
                .Sum(person => person.Happiness));
            //total decay due to population size vs housing
            float loss = averageHappiness * densityHappinessDecay;
            _totalHappiness += averageHappiness - loss;
        }

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