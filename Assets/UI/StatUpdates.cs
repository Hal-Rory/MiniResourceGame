using System;
using Controllers;
using Interfaces;
using TMPro;
using Town.TownPopulation;
using UnityEngine;
using Utility;

public class StatUpdates : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _happinessUpdate;
    [SerializeField] private TextMeshProUGUI _incomeUpdate;
    [SerializeField] private Animator _happinessAnimation;
    [SerializeField] private Animator _incomeAnimation;

    private IncomeManager _income => GameController.Instance.Income;
    private GameTimeManager _time => GameController.Instance.GameTime;
    private TownPopulaceManager _populace => GameController.Instance.TownPopulace;

    private void Start()
    {
        _income.OnIncomeChanged += OnIncomeChanged;
        _time.RegisterListener(clockUpdate: OnClockUpdate);
    }

    private void OnDisable()
    {
        _happinessUpdate.transform.localScale = Vector3.zero;
        _incomeUpdate.transform.localScale = Vector3.zero;
    }

    public void PopupHappiness(string value)
    {
        _happinessUpdate.text = value;
        _happinessAnimation.SetTrigger("popup");
    }

    public void PopupIncome(string value)
    {
        _incomeUpdate.text = value;
        _incomeAnimation.SetTrigger("popup");
    }

    private void OnIncomeChanged(int delta)
    {
        // switch (delta)
        // {
        //     case > 0:
        //         PopupIncome($"Income: {PositiveColor("+$" + delta)}");
        //         break;
        //     case < 0:
        //         PopupIncome($"Income: {NegativeColor("$" +delta)}");
        //         break;
        //     default:
        //         PopupIncome($"Income: {NeutralColor("$+0")}");
        //         break;
        // }
    }

    private void OnClockUpdate(int tick)
    {
        switch (_populace.LastHappinessUpdate)
        {
            case > 0:
                PopupHappiness($"Happiness: {PositiveColor("+" + _populace.LastHappinessUpdate)}");
                break;
            case < 0:
                PopupHappiness($"Happiness: {NegativeColor("-" + _populace.LastHappinessUpdate)}");
                break;
            default:
                PopupHappiness($"Happiness: {NeutralColor("+0")}");
                break;
        }

        //if(_income.NetIncome == 0) PopupIncome($"Income: {NeutralColor("$+0")}");
        switch (_income.NetIncome)
        {
            case > 0:
                PopupIncome($"Income: {PositiveColor("+$" + _income.NetIncome)}");
                break;
            case < 0:
                PopupIncome($"Income: {NegativeColor("$" +_income.NetIncome)}");
                break;
            default:
                PopupIncome($"Income: {NeutralColor("+$0")}");
                break;
        }
    }

    private string PositiveColor(string value)
    {
        return $"<color=#{ColorPaletteUtilities.GreenHex}>{value}</color>";
    }

    private string NegativeColor(string value)
    {
        return $"<color=#{ColorPaletteUtilities.RedHex}>{value}</color>";
    }

    private string NeutralColor(string value)
    {
        return $"<color=#{ColorPaletteUtilities.TanHex}>{value}</color>";
    }
}