using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private List<ITimeListener> _listeners;
    private DateTime _gameTime;
    private event Action<int> _onClockUpdate;
    private Coroutine _clockUpdating;
    [SerializeField] private float _tickDelta;
    public float TimeMultiplier;
    public bool TimeActive { get; private set; }

    public void SetTimeActive(bool active)
    {
        TimeActive = active;
        if (active && _clockUpdating == null)
        {
            _clockUpdating = StartCoroutine(Tick());
        }
    }

    private IEnumerator Tick()
    {
        while (TimeActive)
        {
            yield return new WaitForSeconds(_tickDelta * TimeMultiplier);
            if (_gameTime.AddHours(1).Day != _gameTime.Day)
            {
                //new day
            }
            _gameTime = _gameTime.AddHours(1);
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
}