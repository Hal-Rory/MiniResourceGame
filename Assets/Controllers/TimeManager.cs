using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private List<ITimeListener> _listeners;
    private DateTime _gameTime;
    [SerializeField] private string _startDateTime;
    private event Action<int> _onClockUpdate;
    private Coroutine _clockUpdating;
    [SerializeField] private float _tickDelta;
    public float TimeMultiplier;
    public bool TimeActive { get; private set; }

    private void Start()
    {
        _gameTime = DateTime.Parse(_startDateTime);
    }

    public void SetTimeActive(bool active)
    {
        TimeActive = active;
        if (active && _clockUpdating == null)
        {
            _clockUpdating = StartCoroutine(Tick());
        }
    }

    public float GetClockSpeed()
    {
        return _tickDelta * TimeMultiplier;
    }

    private IEnumerator Tick()
    {
        while (TimeActive)
        {
            yield return new WaitForSeconds(_tickDelta * TimeMultiplier);
            if (_gameTime.AddDays(1).Day != _gameTime.Day)
            {
                //new day
            }
            _gameTime = _gameTime.AddDays(1);
            _onClockUpdate?.Invoke(1);
        }

        _clockUpdating = null;
    }

    public void RegisterListener(ITimeListener listener)
    {
        _onClockUpdate += listener.ClockUpdate;
    }

    public void UnregisterListener(ITimeListener listener)
    {
        _onClockUpdate -= listener.ClockUpdate;
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