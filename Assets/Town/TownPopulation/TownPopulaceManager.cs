using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Controllers;
using Interfaces;
using Placement;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Town.TownPopulation
{
    [Serializable]
    public class TownPopulaceManager : IControllable, IActionBoxHolder
    {
        private float _totalHappiness;
        private int _totalPopulation;
        private int _growthThreshold;
        private List<int> _availableHousing;
        [SerializeField] private float _housingWait;
        private Coroutine _checkForHousingAllowed;
        private ActionBox _housingCoroutine;
        private bool _stagnantGrowth;

        private PopulationFactory _populationFactory => GameController.Instance.Population;
        private IncomeManager _incomeManager => GameController.Instance.Income;

        public void SetUp()
        {
            GameController.Instance.GameTime.RegisterListener(earlyClockUpdate: ClockUpdate);
            GameController.Instance.RegisterPlacementListener(OnNewLot, OnRemoveLot);
            _availableHousing = new List<int>();
            if (_housingCoroutine != null) return;
            _housingCoroutine = new ActionBox(() =>
                {
                    _housingCoroutine.Running = _checkForHousingAllowed == null;
                    _checkForHousingAllowed ??= GameController.Instance.StartCoroutine(CheckHousingCO());
                },
                () =>
                {
                    _housingCoroutine.Running = false;
                    if (_checkForHousingAllowed == null || !GameController.Instance) return;
                    GameController.Instance.StopCoroutine(_checkForHousingAllowed);
                    _checkForHousingAllowed = null;
                }
            );
            GameController.Instance.PickupAction(this, _housingCoroutine);
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
            return !_stagnantGrowth && _growthThreshold >= _populationFactory.GetActivePopulation().Count && happinessValue <= averageHappiness;
        }

        public void SetStagnant(bool stagnant)
        {
            _stagnantGrowth = stagnant;
        }

        public int GetHappiness()
        {
            return (int)_totalHappiness;
        }

        public void AddHousing(int id)
        {
            _availableHousing.Add(id);
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
            if (_checkForHousingAllowed == null)
            {
                _housingCoroutine.Start();
            }
        }

        private IEnumerator CheckHousingCO()
        {
            yield return new WaitForSeconds(_housingWait);
            _checkForHousingAllowed = null;
            _housingCoroutine.Stop();
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
            if (_checkForHousingAllowed != null || !CanPopulationGrow() || _availableHousing.Count == 0) return;
            int nextAvailable = _availableHousing.GetRandomIndex();
            House house = GameController.Instance.LotFactory.GetLot(nextAvailable) as House;
            _availableHousing.Remove(nextAvailable);
            GameController.Instance.Population.CreateHome(house);
        }

        public void PickingUp()
        {
        }

        public void PuttingDown()
        {
            _housingCoroutine.Stop();
        }
    }
}