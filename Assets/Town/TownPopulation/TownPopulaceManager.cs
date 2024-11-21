using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Interfaces;
using Placement;
using UnityEngine;
using Utility;

namespace Town.TownPopulation
{
    [Serializable]
    public class TownPopulaceManager : IControllable, IActionBoxHolder
    {
        private float _totalHappiness;
        private int _totalPopulation;
        [SerializeField] private float _growthThreshold;
        private List<int> _availableHousing;
        [SerializeField] private float _housingWait;
        private Coroutine _checkForHousingAllowed;
        private ActionBox HousingCoroutine;

        private PopulationFactory _populationFactory => GameController.Instance.Population;
        private IncomeManager _incomeManager => GameController.Instance.Income;

        public bool CanPopulationGrow()
        {
            return (_incomeManager.NetIncome + _totalHappiness) / 2 >= _growthThreshold;
        }

        public void SetUp()
        {
            GameController.Instance.GameTime.RegisterListener(earlyClockUpdate: ClockUpdate);
            GameController.Instance.RegisterPlacementListener(OnNewLot, OnRemoveLot);
            _availableHousing = new List<int>();
            if (HousingCoroutine != null) return;
            HousingCoroutine = new ActionBox(() =>
                {
                    HousingCoroutine.Running = _checkForHousingAllowed == null;
                    _checkForHousingAllowed ??= GameController.Instance.StartCoroutine(CheckHousingCO());
                },
                () =>
                {
                    HousingCoroutine.Running = false;
                    if (_checkForHousingAllowed == null || !GameController.Instance) return;
                    GameController.Instance.StopCoroutine(_checkForHousingAllowed);
                    _checkForHousingAllowed = null;
                }
            );
            GameController.Instance.PickupAction(this, HousingCoroutine);
        }

        public void SetDown()
        {
            GameController.Instance.GameTime.UnregisterListener(earlyClockUpdate: ClockUpdate);
            GameController.Instance.UnregisterPlacementListener(OnNewLot, OnRemoveLot);
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
                HousingCoroutine.Start();
            }
        }

        private IEnumerator CheckHousingCO()
        {
            yield return new WaitForSeconds(_housingWait);
            _checkForHousingAllowed = null;
            HousingCoroutine.Stop();
        }

        private void AdjustStockpiles()
        {
            _totalHappiness = _populationFactory.Population.Sum(person => person.Happiness);
        }

        private void CheckHouseholdAvailability()
        {
            if (_checkForHousingAllowed != null || !CanPopulationGrow() || _availableHousing.Count == 0) return;
            int nextAvailable = _availableHousing.GetRandomIndex();
            House house = GameController.Instance.TownLot.GetLot(nextAvailable) as House;
            _availableHousing.Remove(nextAvailable);
            GameController.Instance.Population.CreateHome(house);
        }

        public void PickingUp()
        {
        }

        public void PuttingDown()
        {
            HousingCoroutine.Stop();
        }
    }
}