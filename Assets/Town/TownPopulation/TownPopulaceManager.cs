using System;
using System.Collections.Generic;
using System.Linq;
using Common.Utility;
using Town.TownPopulation;
using UnityEngine;

[Serializable]
public class TownPopulaceManager : IControllable, ITimeListener
{

    private float _totalContentedness;
    private int _totalPopulation;
    [SerializeField] private float _growthThreshold;
    private List<int> _availableHousing;

    private PopulationFactory _populationFactory => GameController.Instance.Population;
    private IncomeManager _incomeManager => GameController.Instance.Income;


    public bool CanPopulationGrow()
    {
        return (_incomeManager.NetIncome + _totalContentedness) / 2 >= _growthThreshold;
    }

    public void SetUp()
    {
        GameController.Instance.GameTime.RegisterListener(this, true);
        GameController.Instance.RegisterPlacementListener(OnNewLot, OnRemoveLot);
        _availableHousing = new List<int>();
    }

    public void SetDown()
    {
        GameController.Instance.GameTime.UnregisterListener(this, true);
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
    }

    private void AdjustStockpiles()
    {
        _totalContentedness = _populationFactory.Population.Sum(person => person.Contentedness);
    }

    private void CheckHouseholdAvailability()
    {
        if (!CanPopulationGrow() || _availableHousing.Count == 0) return;
        int nextAvailable = _availableHousing.GetRandomIndex();
        House house = GameController.Instance.TownLot.GetLot(nextAvailable) as House;
        _availableHousing.Remove(nextAvailable);
        GameController.Instance.Population.CreateHome(house);
    }
}