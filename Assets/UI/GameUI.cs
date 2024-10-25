using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private TimeManager _timeManager => GameController.Instance.TimeManager;
    private MoneyManager _moneyManager => GameController.Instance.MoneyManager;
    private PopulationFactory _populationFactory => GameController.Instance.PopulationFactory;
    public Text CurrentDate;
    public Text CurrentPopulation;
    public Text CurrentIncome;
    private int _currentHousing;

    public GameObject Controls;
    public GameObject ControlsButton;

    public RebindActionUI[] _bindings;

    private void OnEnable()
    {
        _populationFactory.OnPopulationChanged += CheckHomeless;
    }

    private void OnDisable()
    {
        if(GameController.Instance) _populationFactory.OnPopulationChanged -= CheckHomeless;
    }

    public void CheckHomeless()
    {
        _currentHousing = _populationFactory.PopulationHouseholds.Sum(household => household.Homeless ? 1 : 0);
    }

    private void Update()
    {
        CurrentDate.text = _timeManager.GetDate();
        CurrentPopulation.text = $"Current Population: {_populationFactory.Population.Count} Homeless: {_currentHousing}";

        CurrentIncome.text = $"{_moneyManager.CurrentIncome}({_moneyManager.CurrentIncomeTotal:+0;-#})";
    }

    public void SetControls_Button(bool enable)
    {
        Controls.SetActive(enable);
        ControlsButton.SetActive(!enable);
    }

    public void UpdateControls(PlayerInput controls)
    {
        foreach (RebindActionUI ui in _bindings)
        {
            InputBinding[] primBindings = controls.actions[ui.name].bindings.ToArray();
            InputBinding binding = Array.Find(primBindings, binding => binding.groups.Contains(controls.currentControlScheme));
            Guid id = binding.id;
            ui.bindingId = id.ToString();
        }
    }
}