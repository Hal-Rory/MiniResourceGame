using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private TimeManager _timeManager => GameController.Instance.TimeManager;
    private MoneyManager _moneyManager => GameController.Instance.MoneyManager;
    public Text CurrentDate;
    public Text CurrentTime;
    public Text CurrentIncome;

    private void Update()
    {
        CurrentDate.text = _timeManager.GetDate();
        CurrentTime.text = _timeManager.GetTime();

        CurrentIncome.text = $"{_moneyManager.CurrentIncome}(+{_moneyManager.CurrentIncomeTotal})";
    }
}