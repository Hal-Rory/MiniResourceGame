using System;
using System.Linq;
using Town.TownPopulation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private GameTimeManager _gameTime => GameController.Instance.GameTime;
    private MoneyControllable _money => GameController.Instance.Money;
    private PopulationFactory _population => GameController.Instance.Population;
    public Text CurrentDate;
    public Text CurrentPopulation;
    public Text CurrentIncome;
    private int _currentHousing;

    public GameObject Controls;
    public GameObject ControlsButton;

    public RebindActionUI[] _bindings;

    private void OnEnable()
    {
        _population.OnPopulationChanged += PopulationCheck;
    }

    private void OnDisable()
    {
        if(GameController.Instance) _population.OnPopulationChanged -= PopulationCheck;
    }

    public void PopulationCheck()
    {
        _currentHousing = _population.PopulationHouseholds.Sum(household => household.Homeless ? 1 : 0);
    }

    private void Update()
    {
        CurrentDate.text = _gameTime.GetDate();
        CurrentPopulation.text = $"Current Population: {_population.Population.Count} Homeless: {_currentHousing}";

        CurrentIncome.text = $"{_money.CurrentIncome}({_money.CurrentIncomeTotal:+0;-#})";
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