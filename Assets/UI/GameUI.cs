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
    private IncomeManager _income => GameController.Instance.Income;
    private PopulationFactory _population => GameController.Instance.Population;
    public Text CurrentDate;
    public Text CurrentPopulation;
    public Text CurrentIncome;

    public GameObject Controls;
    public GameObject ControlsButton;

    public RebindActionUI[] _bindings;

    private void Update()
    {
        CurrentDate.text = _gameTime.GetDate();
        CurrentPopulation.text = $"Current Population: {_population.Population.Count}";

        CurrentIncome.text = $"{_income.CurrentFunds}({_income.NetIncome:+0;-#})";
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