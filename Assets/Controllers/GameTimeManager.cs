using System;
using System.Collections;
using Controllers;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{

    /// <summary>
    /// The time states of the day that determine person locations
    /// </summary>
    public enum TimesOfDay
    {
        Morning,
        Work,
        Relax,
        Rest
    }

    private DateTime _gameTime;
    private bool _timeActive { get; set; }

    [SerializeField] private string _startDateTime;
    /// <summary>
    /// The first round of updates, typically for managers
    /// </summary>
    private Action<int> _onEarlyClockUpdate;
    /// <summary>
    /// The update for most objects
    /// </summary>
    private Action<int> _onClockUpdate;
    /// <summary>
    /// The last round of updates typically for UI
    /// </summary>
    private Action<int> _onLateClockUpdate;

    /// <summary>
    /// The change in the time of day, the first round for objects
    /// </summary>
    private Action<TimesOfDay> _onClockStateUpdate;

    /// <summary>
    /// The change in the time of day, the last round of updates
    /// </summary>
    private Action<TimesOfDay> _onLateClockStateUpdate;

    /// <summary>
    /// The actual tick coroutine that is stopped and started based on the time controls
    /// </summary>
    private Coroutine _clockUpdating;

    /// <summary>
    /// How often does the clock update
    /// </summary>
    [SerializeField] private float _tickDelta;
    /// <summary>
    /// How fast should that update being going
    /// </summary>
    public int TimeMultiplier { get; private set; }
    /// <summary>
    /// The available multipliers
    /// </summary>
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

    /// <summary>
    /// If time is active, wait for the (tick = 1 hour), send the clock updates at (update = 1 day),
    /// and the state updates at (<see cref="TimeOfDay"/>
    /// </summary>
    /// <returns></returns>
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