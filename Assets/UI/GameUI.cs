using System;
using Controllers;
using Interfaces;
using TMPro;
using Town.TownObjectManagement;
using Town.TownObjects;
using Town.TownPopulation;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;
using Utility;

public class GameUI : MonoBehaviour
{
    private GameTimeManager _gameTime => GameController.Instance.GameTime;
    private IncomeManager _income => GameController.Instance.Income;
    private TownPopulaceManager _populace => GameController.Instance.TownPopulace;
    private PopulationFactory _population => GameController.Instance.Population;
    private WorkplaceManager _workplace => GameController.Instance.Workplace;
    public TextMeshProUGUI CurrentTimeOfDay;
    public TextMeshProUGUI CurrentDate;
    public TextMeshProUGUI CurrentPopulation;
    public TextMeshProUGUI CurrentUnemployed;
    public TextMeshProUGUI CurrentIncome;
    public TextMeshProUGUI CurrentHappiness;
    [SerializeField] private int _abbreviationMax;
    public RebindActionUI[] _bindings;
    [SerializeField] private GameObject _secondaryInfo;
    [SerializeField] private Toggle _secondaryPanelToggle;
    [SerializeField] private Toggle[] _timeToggles;

    [SerializeField] private StatUpdates _stats;

    private SoundManager _soundManager => GameController.Instance.Sound;

    private void Start()
    {
        CurrentPopulation.text = $"{_population.PopulationCount.Abbreviate(_abbreviationMax)}";
        CurrentIncome.text = $"{_income.CurrentFunds.Abbreviate(_abbreviationMax, trailingDigitsCount:2)} ({_income.NetIncome.Abbreviate(_abbreviationMax, "+0;-#")})";
        CurrentHappiness.text = $"{_populace.GetHappiness().Abbreviate(_abbreviationMax, trailingDigitsCount:2)}";
        _income.OnIncomeChanged += UpdateIncome;
    }

    private void Update()
    {
        CurrentDate.text = $"{_gameTime.GetDate()} @ {_gameTime.GetTime()}";
        CurrentTimeOfDay.text = $"{_gameTime.TimeOfDay}";
        if(_secondaryInfo.activeSelf)
        {
            CurrentHappiness.text = $"{_populace.GetHappiness().Abbreviate(_abbreviationMax, trailingDigitsCount: 2)}";
            CurrentPopulation.text = $"{_population.PopulationCount.Abbreviate(_abbreviationMax)}";
            CurrentUnemployed.text = $"{_workplace.TotalWorkforce}/{_population.PopulationCount}";
        }
        _secondaryPanelToggle.interactable = !GameController.Instance.PlacementMode;
        switch (GameController.Instance.PlacementMode)
        {
            case true when _secondaryInfo.activeSelf:
                _secondaryPanelToggle.SetIsOnWithoutNotify(false);
                _secondaryInfo.SetActive(false);
                _stats.gameObject.SetActive(true);
                break;
            case false:
                switch (GameController.Instance.Input.Movement.y)
                {
                    case > 0:
                        SetPanelActive_Toggle(false);
                        _secondaryPanelToggle.SetIsOnWithoutNotify(false);
                        break;
                    case < 0:
                        SetPanelActive_Toggle(true);
                        _secondaryPanelToggle.SetIsOnWithoutNotify(true);
                        break;
                }
                break;
        }

        if (!GameController.Instance.Input.TimePressed) return;
        GameController.Instance.GameTime.IncrementSpeed();
        _timeToggles[GameController.Instance.GameTime.TimeMultiplier].SetIsOnWithoutNotify(true);
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

    private void UpdateIncome(int i)
    {
        CurrentIncome.text = $"${_income.CurrentFunds.Abbreviate(_abbreviationMax, trailingDigitsCount:2)} (+${_income.NetIncome.Abbreviate(_abbreviationMax, trailingDigitsCount:2)})";
    }

    public void SetPanelActive_Toggle(bool active)
    {
        switch (active)
        {
            case true when !_secondaryInfo.activeSelf:
                _soundManager.PlaySelect();
                break;
            case false when _secondaryInfo.activeSelf:
                _soundManager.PlayCancel();
                break;
        }
        _secondaryInfo.SetActive(active);
        _stats.gameObject.SetActive(!active);
    }
}