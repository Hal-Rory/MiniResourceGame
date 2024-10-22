using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private TimeManager _timeManager => GameController.Instance.TimeManager;
    private MoneyManager _moneyManager => GameController.Instance.MoneyManager;
    private PopulationManager _populationManager => GameController.Instance.PopulationManager;
    public Text CurrentDate;
    public Text CurrentPopulation;
    public Text CurrentIncome;
    private int _currentHousing;

    private void OnEnable()
    {
        _populationManager.OnPopulationChanged += CheckHomeless;
    }

    private void OnDisable()
    {
        if(_populationManager) _populationManager.OnPopulationChanged -= CheckHomeless;
    }

    public void CheckHomeless()
    {
        _currentHousing = _populationManager.PopulationHouseholds.Sum(household => household.Homeless ? 1 : 0);
    }

    private void Update()
    {
        CurrentDate.text = _timeManager.GetDate();
        CurrentPopulation.text = $"Current Population: {_populationManager.Population.Count} Homeless: {_currentHousing}";

        CurrentIncome.text = $"{_moneyManager.CurrentIncome}({_moneyManager.CurrentIncomeTotal:+0;-#})";
    }
}