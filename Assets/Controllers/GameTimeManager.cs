using System;
using System.Collections;
using Controllers;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    public enum TimesOfDay
    {
        Morning,
        Work,
        Relax,
        Rest
    }
    private DateTime _gameTime;
    [SerializeField] private string _startDateTime;
    private Action<int> _onEarlyClockUpdate;
    private Action<int> _onClockUpdate;
    private Action<int> _onLateClockUpdate;
    private Action<TimesOfDay> _onClockStateUpdate;
    private Action<TimesOfDay> _onLateClockStateUpdate;
    private Coroutine _clockUpdating;
    [SerializeField] private float _tickDelta;
    public int TimeMultiplier { get; private set; }

    private bool _timeActive { get; set; }

    private static readonly int[] _timeSpeeds =
    {
        0, 1, 10
    };

    public TimesOfDay TimeOfDay
    {
        get
        {
            return _gameTime.Hour switch
            {
                >= 5 and < 9 => TimesOfDay.Morning,
                >= 9 and < 17 => TimesOfDay.Work,
                >= 17 and < 23 => TimesOfDay.Relax,
                < 5 or >= 23 => TimesOfDay.Rest
            };
        }
    }

    private void Start()
    {
        _gameTime = DateTime.Parse(_startDateTime);
    }

    public void SetTimeActive(bool active)
    {
        _timeActive = active;
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

    public void IncrementSpeed()
    {
        SetMultiplier((TimeMultiplier + 1 + _timeSpeeds.Length) % _timeSpeeds.Length);
    }

    public void SetMultiplier(int index)
    {
        TimeMultiplier = index;
        SetTimeActive(false);
        SetTimeActive(true);
    }

    public void StopTime()
    {
        SetTimeActive(false);
    }

    public float GetClockSpeed()
    {
        return _tickDelta * _timeSpeeds[TimeMultiplier];
    }

    public float GetTimePercentage()
    {
        return _gameTime.Hour / 23f;
    }

    private IEnumerator Tick()
    {
        while (_timeActive)
        {
            if (TimeMultiplier != 0)
            {
                yield return new WaitForSeconds(_tickDelta / _timeSpeeds[TimeMultiplier]);
                if (_gameTime.AddHours(1).Day != _gameTime.Day)
                {
                    _onEarlyClockUpdate?.Invoke(1);
                    _onClockUpdate?.Invoke(1);
                    _onLateClockUpdate?.Invoke(1);
                }

                TimesOfDay timeOfDay = TimeOfDay;
                _gameTime = _gameTime.AddHours(1);
                if (timeOfDay != TimeOfDay)
                {
                    _onClockStateUpdate?.Invoke(TimeOfDay);
                    _onLateClockStateUpdate?.Invoke(TimeOfDay);
                }
            } else yield return null;
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
        return _gameTime.ToString("MMM dd");
    }

    public string GetTime()
    {
        return _gameTime.ToShortTimeString();
    }
}