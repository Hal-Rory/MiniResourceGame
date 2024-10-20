using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    public PopulationManager PopulationManager;
    public InputManager InputManager;
    public ObjectManager ObjectManager;
    public WorkplaceManager WorkplaceManager;
    public ObjectPlacer ObjectPlacer;
    public TimeManager TimeManager;
    public MoneyManager MoneyManager;

    private int _population;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        TimeManager.SetTimeActive(true);
    }

    public TownLot GetLot(int id)
    {
        return ObjectPlacer.GetLot(id);
    }

    public List<T> GetLots<T>(string id)
    {
        return ObjectPlacer.GetLots<T>(id);
    }

    public List<T> GetLots<T>(int[] ids)
    {
        return ObjectPlacer.GetLots<T>(ids);
    }

    public void RegisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) ObjectPlacer.OnLotAdded += creationListener;
        if(destructionListener != null) ObjectPlacer.OnLotRemoved += destructionListener;
    }

    public void UnregisterPlacementListener(Action<TownLot> creationListener = null, Action<TownLot> destructionListener = null)
    {
        if(creationListener != null) ObjectPlacer.OnLotAdded -= creationListener;
        if(destructionListener != null) ObjectPlacer.OnLotRemoved -= destructionListener;
    }
}