using System;
using System.Collections;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public enum TimesOfDay
    {
        Work,
        Relax,
        Rest
    }
    private DateTime _gameTime;
    [SerializeField] private string _startDateTime;
    [SerializeField] private string _shortDate;
    private Action<int> _onEarlyClockUpdate;
    private Action<int> _onClockUpdate;
    private Action<int> _onLateClockUpdate;
    private Action<TimesOfDay> _onClockStateUpdate;
    private Action<TimesOfDay> _onLateClockStateUpdate;
    private Coroutine _clockUpdating;
    [SerializeField] private float _tickDelta;
    public float TimeMultiplier;
    public bool TimeActive { get; private set; }

    public TimesOfDay TimeOfDay
    {
        get
        {
            return _gameTime.Hour switch
            {
                >= 7 and < 16 => TimesOfDay.Work,
                >= 16 and < 22 => TimesOfDay.Relax,
                < 7 or >= 22 => TimesOfDay.Rest
            };
        }
    }

    private void Start()
    {
        _gameTime = DateTime.Parse(_startDateTime);
    }

    public void SetTimeActive(bool active)
    {
        TimeActive = active;
        switch (active)
        {
            case true when _clockUpdating == null:
                _clockUpdating = StartCoroutine(Tick());
                break;
            case false when _clockUpdating != null:
                StopCoroutine(_clockUpdating);
                _clockUpdating = null;
                break;
        }
    }

    public void SetMultiplier(float multiplier)
    {
        TimeMultiplier = multiplier;
    }

    public float GetClockSpeed()
    {
        return _tickDelta * TimeMultiplier;
    }

    public float GetTimePercentage()
    {
        return _gameTime.Hour / 23f;
    }

    private IEnumerator Tick()
    {
        while (TimeActive)
        {
            yield return new WaitForSeconds(_tickDelta * TimeMultiplier);
            if (_gameTime.AddHours(1).Day != _gameTime.Day)
            {
                _onEarlyClockUpdate?.Invoke(1);
                _onClockUpdate?.Invoke(1);
                _onLateClockUpdate.Invoke(1);
            }

            TimesOfDay timeOfDay = TimeOfDay;
            _gameTime = _gameTime.AddHours(1);
            if (timeOfDay != TimeOfDay)
            {
                _onClockStateUpdate?.Invoke(TimeOfDay);
                _onLateClockStateUpdate?.Invoke(TimeOfDay);
            }
            _shortDate = GetTime();
        }

        _clockUpdating = null;
    }

    public void RegisterListener(
        Action<int> earlyClockUpdate = null,
        Action<int> clockUpdate = null,
        Action<int> lateClockUpdate = null,
        Action<TimesOfDay> stateClockUpdate = null,
        Action<TimesOfDay> lateStateClockUpdate = null)
    {
        if(earlyClockUpdate != null) _onEarlyClockUpdate += earlyClockUpdate;
        if(clockUpdate != null) _onClockUpdate += clockUpdate;
        if(lateClockUpdate != null) _onLateClockUpdate += lateClockUpdate;
        if(stateClockUpdate != null) _onClockStateUpdate += stateClockUpdate;
        if(lateStateClockUpdate != null) _onLateClockStateUpdate += lateStateClockUpdate;
    }

    public void UnregisterListener(
        Action<int> earlyClockUpdate = null,
        Action<int> clockUpdate = null,
        Action<int> lateClockUpdate = null,
        Action<TimesOfDay> stateClockUpdate = null,
        Action<TimesOfDay> lateStateClockUpdate = null)
    {
        if(earlyClockUpdate != null) _onEarlyClockUpdate -= earlyClockUpdate;
        if(clockUpdate != null) _onClockUpdate -= clockUpdate;
        if(lateClockUpdate != null) _onLateClockUpdate += lateClockUpdate;
        if(stateClockUpdate != null) _onClockStateUpdate -= stateClockUpdate;
        if(lateStateClockUpdate != null) _onLateClockStateUpdate -= lateStateClockUpdate;
    }

    public string GetDate()
    {
        return _gameTime.ToShortDateString();
    }

    public string GetTime()
    {
        return _gameTime.ToShortTimeString();
    }
}