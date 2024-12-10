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
    private TownPopulaceManager _populace => GameController.Instance.TownPopulace;
    private PopulationFactory _population => GameController.Instance.Population;
    private TownObjectManager _town => GameController.Instance.TownObject;
    public TextMeshProUGUI CurrentDate;
    public TextMeshProUGUI CurrentPopulation;
    public TextMeshProUGUI CurrentIncome;
    public TextMeshProUGUI CurrentHappiness;
    [SerializeField] private int _abbreviationMax;
    public RebindActionUI[] _bindings;
    [SerializeField] private GameObject _secondaryInfo;
    [SerializeField] private Toggle _secondaryPanelToggle;

    private SoundManager _soundManager => GameController.Instance.Sound;

    private void Start()
    {
        CurrentPopulation.text = $"{_population.GetActivePopulationCountString().Abbreviate(_abbreviationMax)}";
        CurrentIncome.text = $"{_income.CurrentFunds.Abbreviate(_abbreviationMax, trailingDigitsCount:2)} ({_income.NetIncome.Abbreviate(_abbreviationMax, "+0;-#")})";
        CurrentHappiness.text = $"{_populace.GetHappiness().Abbreviate(_abbreviationMax, trailingDigitsCount:2)}";
        _population.OnPopulationChanged += OnPopulationChanged;
        _income.OnIncomeChanged += AnimateIncome;
        _town.OnStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(bool changed)
    {
        switch (changed)
        {
            case true when _secondaryInfo.activeSelf:
                _secondaryPanelToggle.SetIsOnWithoutNotify(false);
                _secondaryPanelToggle.interactable = false;
                _secondaryInfo.SetActive(false);
                break;
            case false:
                _secondaryPanelToggle.interactable = true;
                break;
        }
    }

    private void OnPopulationChanged(IPopulation obj)
    {
        CurrentPopulation.text = $"{_population.GetActivePopulationCountString().Abbreviate(_abbreviationMax)}";
    }

    private void Update()
    {
        CurrentDate.text = $"{_gameTime.GetDate()} @ {_gameTime.GetTime()}";
        CurrentHappiness.text = $"{_populace.GetHappiness().Abbreviate(_abbreviationMax, trailingDigitsCount:2)}";
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
        CurrentIncome.text = $"{_income.CurrentFunds.Abbreviate(_abbreviationMax, trailingDigitsCount:2)} ({_income.NetIncome.Abbreviate(_abbreviationMax, "+0;-#")})";
    }

    public void SetPanelActive_Toggle(bool active)
    {
        if(active) _soundManager.PlaySelect();
        else _soundManager.PlayCancel();
        _secondaryInfo.SetActive(active);
    }
}