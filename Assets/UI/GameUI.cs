using System;
using Controllers;
using Interfaces;
using TMPro;
using Town.TownPopulation;
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
    public TextMeshProUGUI CurrentIncome;
    public Animator CurrentIncomeAnimation;

    public GameObject Controls;
    public GameObject ControlsButton;

    public RebindActionUI[] _bindings;

    private void Start()
    {
        OnPopulationChanged(null);
        AnimateIncome(0);
        _population.OnPopulationChanged += OnPopulationChanged;
        GameController.Instance.Income.OnIncomeChanged += AnimateIncome;
    }

    private void OnPopulationChanged(IPopulation obj)
    {
        CurrentPopulation.text = $"{_population.GetActivePopulationCountString().Abbreviate()}";
    }

    private void Update()
    {
        CurrentDate.text = _gameTime.GetDate();
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

    private void AnimateIncome(int i)
    {
        CurrentIncome.text = $"{_income.CurrentFunds.Abbreviate(trailingDigitsCount:2)} ({_income.NetIncome.Abbreviate("0;-#")})";
        CurrentIncomeAnimation.Play(i > 0 ? "Gain" : "Loss");
    }
}